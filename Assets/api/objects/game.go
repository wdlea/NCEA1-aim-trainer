package objects

import (
	"fmt"
	"time"

	ll "github.com/wdlea/GOGenericLinkedList"
)

type GameState int

const TICK_RATE = 40 //hz
const TICK_INTERVAL = time.Second / TICK_RATE
const TICK_INTERVAL_SECONDS = 1.0 / TICK_RATE

const GAME_DURATION = 5 * time.Minute

var ActiveGames *ll.LinkedList[*Game]

func FindGame(name string) *ll.LinkedListNode[*Game] {
	if len(name) == GAME_NAME_LENGTH {
		for game := ActiveGames.First; game != nil; game = game.Next {
			if game.Value.Name == name {
				return game
			}
		}
	} else {
		fmt.Println("Name with invalid length supplied, skipping")
	}

	return nil
}

const (
	GAME_WAITING_FOR_PLAYERS GameState = iota
	GAME_RUNNING
	GAME_DONE
)

// dont put IPs anywhere this can reach, it is marshalled and sent to players
type Game struct {
	Players [2]*Player
	State   GameState
	Host    *Player //avoid circular marshal

	ListEntry *ll.LinkedListNode[*Game] `json:"-"`

	Name string

	Done         chan int     `json:"-"`
	updateTicker *time.Ticker `json:"-"`
}

func (g *Game) RemovePlayer(p *Player) {
	if g.State == GAME_WAITING_FOR_PLAYERS {
		g.removeFromPlayerList(p)
		return
	} else {
		g.Dispose() //cant keep playing with one player
	}

	if g.Players[0] == nil && g.Players[1] == nil { //if no players just destroy the server
		g.Dispose()
	}
}

func (g *Game) StartGame() {
	g.State = GAME_RUNNING
	timer := time.NewTimer(GAME_DURATION)
	go func(C <-chan time.Time, g *Game) {
		for {
			select {
			case <-C:
				g.Done <- 0
				return

			default:
				if len(g.Done) > 0 {
					return
				}
			}
		}
	}(timer.C, g)

	g.updateTicker = time.NewTicker(TICK_INTERVAL)

	go g.run()
}

func (g *Game) run() {
	for g.State == GAME_RUNNING {
		<-g.updateTicker.C //wait for update
		g.Update(TICK_INTERVAL_SECONDS)
	}
}

func (g *Game) Update(deltatime float32) {
	for _, player := range g.Players {
		player.Update(deltatime)
	}
}

// remove all references of this game, resulting in the garbage collector destroying it
func (g *Game) Dispose() {

	g.State = GAME_DONE

	for _, p := range g.Players {
		p.Game = nil //remove all players
	}
	g.Players = [2]*Player{} //clear players, allowing dropped connections to be destroyed by the garbage collector
	g.ListEntry.Pop(ActiveGames)
}

func (g *Game) removeFromPlayerList(p *Player) {
	playerIdx := -1
	for idx, pl := range g.Players {
		if pl == p {
			playerIdx = idx
			break
		}
	}

	if playerIdx != -1 {
		g.Players[playerIdx] = nil
	} else {
		fmt.Println("Tried to remove player not in game")
	}
}

type JoinGameRequest struct {
	Name string //name is used like a password and is not published in any way, rather clients must obtain this from host
}

type HostGameResponse struct {
	Ok   bool
	Name string
}
