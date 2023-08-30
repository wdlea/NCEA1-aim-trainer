package logic

import (
	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// handles the player leaving a game
func HandleLeaveGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	//leave the game
	user.LeaveGame()

	//send a response
	response = append(
		response,
		Packet{
			Type: 'L', //error packet
			Content: []byte(
				"success",
			),
		},
	)
	return
}
