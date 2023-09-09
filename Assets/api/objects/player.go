package objects

import (
	"fmt"
	"net"
)

const GAME_NAME_LENGTH = 8

// represents a player in the game
type Player struct {
	Name string

	Game *Game `json:"-"` //avoid circular marshal

	X, Y, Dx, Dy float64

	Conn net.Conn `json:"-"`

	Score float64
}

// represents an instant of the players motion
type Frame struct {
	X, Y, Dx, Dy float64
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
	fmt.Printf("Player %s applying frame\n", p.Name)

	//add the frame
	p.X, p.Y, p.Dx, p.Dy = f.X, f.Y, f.Dx, f.Dy
}

// updates a player based on theyr dX and dY
func (p *Player) Update(deltaTime float64) {
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
	p.Conn.Write([]byte{BROADCAST_PREFIX})
	p.Conn.Write([]byte{message.Type})
	p.Conn.Write(message.Content)
	p.Conn.Write([]byte{'\n'})
}
