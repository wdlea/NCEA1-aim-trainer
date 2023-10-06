package objects

import (
	"fmt"
	"net"
)

// Represents a player in the game
type Player struct {
	Name string

	Game *Game `json:"-"` //avoid circular marshal

	X, Y, Dx, Dy, DDx, DDy float64

	Conn net.Conn `json:"-"`

	Score float64
}



// Disposes of the player
func (p *Player) Dispose() {
	fmt.Printf("Player %s is being disposed\n", p.Name)
	p.LeaveGame()
}

// Resets the players position
func (p *Player) resetPos() {
	fmt.Printf("Player %s is having thier position reset\n", p.Name)
	p.X, p.Y, p.Dx, p.Dy = 0, 0, 0, 0
}