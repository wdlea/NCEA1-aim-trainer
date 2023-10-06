package objects

import "fmt"

// Leaves the game
func (p *Player) LeaveGame() {
	fmt.Printf("Player %s leaving game\n", p.Name)

	//if the game is not nil
	if p.Game != nil {
		//remove me from the game
		p.Game.RemovePlayer(p)
	}
	p.Game = nil
}