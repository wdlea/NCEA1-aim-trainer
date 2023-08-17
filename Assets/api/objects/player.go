package objects

import (
	"fmt"
)

const GAME_NAME_LENGTH = 8

type Player struct {
	Name string `json:"name"`

	Game *Game `json:"-"` //avoid circular marshal

	X, Y, Dx, Dy float32
}

type Frame struct {
	X, Y, Dx, Dy float32
}

func (p *Player) HostGame() {
	g := new(Game)

	var err error

	g.Name = RandomURLSafeString(GAME_NAME_LENGTH)

	if err != nil {
		fmt.Printf("Error in creating game token: %s", err.Error())
		return
	}

	p.Game = g
	g.Players[0] = p

	p.resetPos()

	g.ListEntry = ActiveGames.AddLast(g)
}

func (p *Player) JoinGame(g *Game) (ok bool) {
	p.LeaveGame()

	ok = g.Players[1] == nil

	if ok {
		g.Players[1] = p

		p.Game = g
	}

	p.resetPos()

	return
}

func (p *Player) LeaveGame() {
	if p.Game != nil {
		p.Game.RemovePlayer(p)
	}
	p.Game = nil
}

func (p *Player) ApplyFrame(f Frame) {
	p.X, p.Y, p.Dx, p.Dy = f.X, f.Y, f.Dx, f.Dy
}

func (p *Player) Update(deltaTime float32) {
	p.X += p.Dx * deltaTime
	p.Y += p.Dy * deltaTime
}

func (p *Player) Dispose() {
	p.LeaveGame()
}

func (p *Player) resetPos() {
	p.X, p.Y, p.Dx, p.Dy = 0, 0, 0, 0
}
