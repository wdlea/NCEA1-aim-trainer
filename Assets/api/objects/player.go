package objects

import (
	"fmt"
	"net"
)

const GAME_NAME_LENGTH = 8

type Player struct {
	Name string

	Game *Game `json:"-"` //avoid circular marshal

	X, Y, Dx, Dy float32

	Conn net.Conn `json:"-"`
}

type Frame struct {
	X, Y, Dx, Dy float32
}

func (p *Player) HostGame() (ok bool) {
	fmt.Printf("Player %s attempting to host game\n", p.Name)

	p.LeaveGame()

	g := new(Game)

	var err error

	g.Name = RandomURLSafeString(GAME_NAME_LENGTH)

	if err != nil {
		fmt.Printf("Error in creating game error: %s\n", err.Error())
		return false
	}

	p.Game = g
	g.Players[0] = p

	p.resetPos()

	g.ListEntry = ActiveGames.AddLast(g)

	return true
}

func (p *Player) JoinGame(g *Game) (ok bool) {
	fmt.Printf("Player %s joining game %s\n", p.Name, g.Name)

	p.LeaveGame()

	ok = g.Players[1] == nil

	if ok {
		g.Players[1] = p

		p.Game = g

		p.resetPos()

		g.StartGame()
	}

	return
}

func (p *Player) LeaveGame() {
	fmt.Printf("Player %s leaving game\n", p.Name)

	if p.Game != nil {
		p.Game.RemovePlayer(p)
	}
	p.Game = nil
}

func (p *Player) ApplyFrame(f Frame) {
	fmt.Printf("Player %s applying frame\n", p.Name)
	p.X, p.Y, p.Dx, p.Dy = f.X, f.Y, f.Dx, f.Dy
}

func (p *Player) Update(deltaTime float32) {
	p.X += p.Dx * deltaTime
	p.Y += p.Dy * deltaTime
}

func (p *Player) Dispose() {
	fmt.Printf("Player %s is being disposed\n", p.Name)
	p.LeaveGame()
}

func (p *Player) resetPos() {
	fmt.Printf("Player %s is having thier position reset\n", p.Name)
	p.X, p.Y, p.Dx, p.Dy = 0, 0, 0, 0
}

const BROADCAST_PREFIX = 'B'

func (p *Player) SendBroadcast(message Packet) {
	p.Conn.Write([]byte{BROADCAST_PREFIX})
	p.Conn.Write([]byte{message.Type})
	p.Conn.Write(message.Content)
	p.Conn.Write([]byte{'\n'})
}
