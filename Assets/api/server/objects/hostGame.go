package objects

import (
	"fmt"

	"github.com/wdlea/aimtrainerAPI/helpers"
)

// Creates and hosts a game
func (p *Player) HostGame() (ok bool) {
	fmt.Printf("Player %s attempting to host game\n", p.Name)

	//leave current game
	p.LeaveGame()

	//create a new one
	g := new(Game)

	//assign a random name
	g.Name = helpers.RandomURLSafeString(GAME_NAME_LENGTH)

	//set the players game and the games players
	p.Game = g
	g.Players[0] = p

	//reset the player position
	p.resetPos()

	//add the game to the gamelist
	g.ListEntry = ActiveGames.AddLast(g)

	return true
}