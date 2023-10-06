package objects

import (
	"fmt"
	"math"
	"net"
)

const GAME_NAME_LENGTH = 8

const VELOCITY_DAMPING_FACTOR = 0.5
const ACCELLERATION_DAMPING_FACTOR = 0.1

// Represents a player in the game
type Player struct {
	Name string

	Game *Game `json:"-"` //avoid circular marshal

	X, Y, Dx, Dy, DDx, DDy float64

	Conn net.Conn `json:"-"`

	Score float64
}

// Updates a player based on theyr dX and dY
func (p *Player) Update(deltaTime float64) {
	DFactor := math.Pow(VELOCITY_DAMPING_FACTOR, deltaTime)

	p.Dx *= DFactor
	p.Dy *= DFactor

	DDFactor := math.Pow(ACCELLERATION_DAMPING_FACTOR, deltaTime)

	p.DDx *= DDFactor
	p.DDy *= DDFactor

	p.Dx += p.DDx * deltaTime
	p.Dy += p.DDy * deltaTime

	p.X += p.Dx * deltaTime
	p.Y += p.Dy * deltaTime
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

const BROADCAST_PREFIX = 'B'

// sends a broadcast to the player
func (p *Player) SendBroadcast(message Packet) {
	p.Conn.Write([]byte(fmt.Sprintf(
		string(BROADCAST_PREFIX)+"%s%s\n",
		string(message.Type),
		message.Content,
	)))
}
