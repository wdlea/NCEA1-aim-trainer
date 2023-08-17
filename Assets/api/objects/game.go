package objects

import (
	"fmt"

	ll "github.com/wdlea/GOGenericLinkedList"
)

type GameState int

var ActiveGames *ll.LinkedList[*Game]

const (
	GAME_WAITING_FOR_PLAYERS GameState = iota
	GAME_RUNNING
	GAME_DONE
)

// dont put IPs anywhere this can reach, it is marshalled and sent to players
type Game struct {
	Players [2]*Player
	State   GameState

	ListEntry ll.LinkedListNode[*Game] `json:"-"`
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
