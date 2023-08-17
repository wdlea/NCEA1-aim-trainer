package api

import (
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
	if game.Value.Started {
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
	return
}
