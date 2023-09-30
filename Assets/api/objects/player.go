package objects

import (
	"fmt"
	"math"
	"net"
)

const GAME_NAME_LENGTH = 8

const VELOCITY_DAMPING_FACTOR = 0.5
const ACCELLERATION_DAMPING_FACTOR = 0.1

// represents a player in the game
type Player struct {
	Name string

	Game *Game `json:"-"` //avoid circular marshal

	X, Y, Dx, Dy, DDx, DDy float64

	Conn net.Conn `json:"-"`

	Score float64
}

// represents an instant of the players motion
type Frame struct {
	X, Y, Dx, Dy, DDx, DDy float64
}

// creates and hosts a game
func (p *Player) HostGame() (ok bool) {
	fmt.Printf("Player %s attempting to host game\n", p.Name)

	//leave current game
	p.LeaveGame()

	//create a new one
	g := new(Game)

	//assign a random name
	g.Name = RandomURLSafeString(GAME_NAME_LENGTH)

	//set the players game and the games players
	p.Game = g
	g.Players[0] = p

	//reset the player position
	p.resetPos()

	//add the game to the gamelist
	g.ListEntry = ActiveGames.AddLast(g)

	return true
}

// joins a game
func (p *Player) JoinGame(g *Game) (ok bool) {
	fmt.Printf("Player %s joining game %s\n", p.Name, g.Name)

	//leave the current game
	p.LeaveGame()

	//make sure there is space for me
	ok = g.Players[1] == nil

	//if there is space
	if ok {

		//set the games player
		g.Players[1] = p

		//and the players game
		p.Game = g

		//reset and start game
		p.resetPos()

		g.StartGame()
	}

	return
}

// leaves the game
func (p *Player) LeaveGame() {
	fmt.Printf("Player %s leaving game\n", p.Name)

	//if the game is not nil
	if p.Game != nil {
		//remove me from the game
		p.Game.RemovePlayer(p)
	}
	p.Game = nil
}

// apply a frame to a pleyer
func (p *Player) ApplyFrame(f Frame) {

	//add the frame
	p.X, p.Y, p.Dx, p.Dy, p.DDx, p.DDy = f.X, f.Y, f.Dx, f.Dy, f.DDx, f.DDy
}

// updates a player based on theyr dX and dY
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

// Disploses of the player
func (p *Player) Dispose() {
	fmt.Printf("Player %s is being disposed\n", p.Name)
	p.LeaveGame()
}

// resets the players position
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
