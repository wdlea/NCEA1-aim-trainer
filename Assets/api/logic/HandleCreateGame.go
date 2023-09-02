package logic

import (
	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// Handles a player creating a game
func HandleCreateGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	//if the player does not have a name send an error
	if len(user.Name) <= 0 {
		response = append(
			response,
			Packet{
				Type: 'E',
				Content: []byte(
					"Cannot create a game without having a name.",
				),
			},
		)
		return
	}

	//send the response
	response = append(
		response,
		Packet{
			Type:    'C',
			Content: []byte(user.Game.Name),
		},
	)

	return
}
