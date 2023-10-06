package objects

import "fmt"

// Joins a game
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