package objects

import (
	"fmt"

	ll "github.com/wdlea/GOGenericLinkedList"
)

type GameState int

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
}

func (g *Game) RemovePlayer(p *Player) {
	if g.State == GAME_WAITING_FOR_PLAYERS {
		g.removeFromPlayerList(p)
		return
	} else {
		g.Dispose() //cant keep playing with one player
	}

	if g.Players[0] == nil && g.Players[1] == nil {
		g.Dispose()
	}
}

func (g *Game) StartGame() {

}

// remove all references of this game, resulting in the garbage collector destroying it
func (g *Game) Dispose() {

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
	Ok  bool
	Name string
}
