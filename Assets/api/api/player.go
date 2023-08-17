package api

import (
	"fmt"
	"time"

	ll "github.com/wdlea/GOGenericLinkedList"
)

type Player struct {
	Pos  Vector2
	DPos Vector2

	LastUpdate time.Time

	CurrentGame *ll.LinkedListNode[*Game]
}

func (p *Player) ApplyFrame(f Frame) {
	p.Pos = f.Pos
	p.DPos = f.DPos
}

func (p *Player) Tick(deltaTime float64) {
	delta := p.DPos.Copy()
	delta.Scale(deltaTime)

	p.Pos.Add(delta)
}

func (p *Player) JoinGame(game *ll.LinkedListNode[*Game], attemptedPassword string) (sucess bool, passwordIncorrect bool, lobbyFull bool, gameStarted bool) {
	if game.Value.State != GAME_STATE_PENDING_PLAYERS {
		gameStarted = true
		return
	}
	if len(game.Value.Password) > 0 && attemptedPassword != game.Value.Password {
		passwordIncorrect = true
		return
	}
	if len(game.Value.Players) > 2 {
		lobbyFull = true
		return
	}

	sucess = true

	p.CurrentGame = game
	return
}

func (p *Player) LeaveGame() {
	if p.CurrentGame != nil {
		playerIndex := -1
		for i, player := range p.CurrentGame.Value.Players {
			if p == player {
				playerIndex = i
				break
			}
		}
		if playerIndex == -1 {
			fmt.Println("Player removed from game they were not in")
			return
		} else {
			if playerIndex < len(p.CurrentGame.Value.Players)-1 {
				p.CurrentGame.Value.Players = append(p.CurrentGame.Value.Players[:playerIndex], p.CurrentGame.Value.Players[playerIndex+1:]...)
			} else {
				p.CurrentGame.Value.Players = p.CurrentGame.Value.Players[:playerIndex]
			}
		}
	}
	p.CurrentGame = nil
}

func (p *Player) CreateGame(settings GameSettings) {
	p.LeaveGame()
}
