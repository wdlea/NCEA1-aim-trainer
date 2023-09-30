package objects

import (
	"fmt"
	"math/rand"
	"strconv"
	"sync"
	"time"

	ll "github.com/wdlea/GOGenericLinkedList"
)

type GameState int

const TICK_RATE = 40 //hz
const TICK_INTERVAL = time.Second / TICK_RATE
const TICK_INTERVAL_SECONDS = 1.0 / TICK_RATE

const TARGET_SPAWN_RATE = 2 //hz

const COUNTDOWN_DURATION = 1200 * time.Millisecond //fast countdown, becuase that looks cooler

const GAME_DURATION = 5 * time.Minute

var ActiveGames *ll.LinkedList[*Game]

// Searches for a game by the name
func FindGame(name string) *ll.LinkedListNode[*Game] {
	//first check if the name is valid
	if len(name) == GAME_NAME_LENGTH {

		//if it is, try a game with it and return it
		for game := ActiveGames.First; game != nil; game = game.Next {
			if game.Value.Name == name {
				return game
			}
		}
	} else {
		//otherwise print an error to console and return nil
		fmt.Println("Name with invalid length supplied, skipping")
	}

	return nil
}

// creating "enums"(as close as you can get to them in GO, a custom type and some constants)
const (
	GAME_WAITING_FOR_PLAYERS GameState = iota
	GAME_PENDING_START                 //TODO make sure that the C# scripts also use this definition
	GAME_RUNNING
	GAME_DONE
)

// dont put IPs anywhere this can reach, it is marshalled and sent to players
type Game struct {
	Players [2]*Player
	State   GameState

	ListEntry *ll.LinkedListNode[*Game] `json:"-"` //`json:"-"` sets the custom name so it will not be marshalled

	Name string

	Targets         ll.LinkedList[*Target] `json:"-"` //dont serialize, the data will be shared using broadcasts
	CurrentTargetID float64                `json:"-"`

	DestructionLock sync.Mutex   `json:"-"`
	Done            chan int     `json:"-"`
	updateTicker    *time.Ticker `json:"-"`
}

// removes a player from the game
func (g *Game) RemovePlayer(p *Player) {

	//if the game has not yet started
	if g.State == GAME_WAITING_FOR_PLAYERS {
		//remove the player
		g.removeFromPlayerList(p)
		return
	} else {
		//if it has just destroy the game
		g.Dispose() //cant keep playing with one player
	}

	if g.Players[0] == nil && g.Players[1] == nil { //if no players just destroy the server
		g.Dispose()
	}
}

// Starts a game
func (g *Game) StartGame() {
	//set state and start the timer
	g.State = GAME_PENDING_START

	g.SendBroadcastAll(Packet{
		Type:    'S',
		Content: []byte(strconv.Itoa(int(COUNTDOWN_DURATION / time.Millisecond))),
	})

	startTimer := time.NewTimer(COUNTDOWN_DURATION)

	//this blocks, I am not using goroutine so that I can wait for this method to finish as a way
	//to indirectly get the status of the game
	g.waitGameStart(startTimer)
}

func (g *Game) waitGameStart(timer *time.Timer) {
	gameTimer := time.NewTimer(GAME_DURATION)
	g.Done = make(chan int, 1)

	//start thread to listen for g.Done and the timer and quit the game once finished
	go listenGameQuit(gameTimer.C, g)

	// setup the ticker for the game updates
	g.updateTicker = time.NewTicker(TICK_INTERVAL)

	<-timer.C //wait for timer
	g.State = GAME_RUNNING

	g.SendBroadcastAll(Packet{
		Type:    'S',
		Content: []byte("-1"), //any value under 0 will be registered as a start
	})

	//actually start the game logic
	go g.run()
}

// listens for the game quit on both channels, then sends the message on the other
func listenGameQuit(C <-chan time.Time, g *Game) {
	for {
		select {
		case <-C:
			fmt.Println("Timer expired, stopping game")
			g.Done <- 0
			return

		default:
			if len(g.Done) > 0 {
				fmt.Println("game stopped not via timeout")
				return
			}
		}
	}
}

// runs a game
func (g *Game) run() {
	for {
		if g.tick() {
			return
		}
	}
}

func (g *Game) tick() (doExit bool) {
	//get lock
	g.DestructionLock.Lock()
	defer g.DestructionLock.Unlock()

	//check if disposed
	if g.State != GAME_RUNNING {
		return true
	}

	select {
	case <-g.updateTicker.C: //wait for update
		g.Update(TICK_INTERVAL_SECONDS) //this is being updated directly after, if the game is disposed between the ops this will panic, look into mutexes to stop this
	case <-g.Done:
		return true
	}

	return false
}

// updates a game, should be called based on the server tick rate
func (g *Game) Update(deltatime float64) {
	for _, player := range g.Players {
		player.Update(deltatime)
	}

	//roll chance, proportional to deltaTime so that the spawn chance is uniform
	if rand.Float64() < TARGET_SPAWN_RATE*deltatime {
		SpawnTarget(g)
	}
}

// remove all references of this game, resulting in the garbage collector destroying it
func (g *Game) Dispose() {
	g.DestructionLock.Lock()
	defer g.DestructionLock.Unlock()

	g.State = GAME_DONE
	g.Done <- 0

	for _, p := range g.Players {
		p.Game = nil //remove all players
	}
	g.Players = [2]*Player{} //clear players, allowing dropped connections to be destroyed by the garbage collector
	g.ListEntry.Pop(ActiveGames)
}

// removes a player from the games player list
func (g *Game) removeFromPlayerList(p *Player) {
	//search for the player using -1 as the nil value
	playerIdx := -1
	for idx, pl := range g.Players {
		if pl == p {
			playerIdx = idx
			break
		}
	}

	//if the player was found remove then
	if playerIdx != -1 {
		g.Players[playerIdx] = nil
	} else {
		//otherwise print an error to console
		fmt.Println("Tried to remove player not in game")
	}
}

// sends a broadcast to all players
func (g *Game) SendBroadcastAll(p Packet) {
	for _, player := range g.Players {
		player.SendBroadcast(p)
	}
}
