package handlers

import (
	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// Handles the 'j' packet, which is sent whenever the player tries to join a game
func HandleJoinGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	//if the name has not been set prevent them from joining
	if len(user.Name) <= 0 {
		response = append(
			response,
			Packet{
				Type: 'E', //error packet
				Content: []byte(
					"Cannot join a game when you do not have a name.",
				),
			},
		)
		return
	}

	//look up the game with the desired name
	game := FindGame(string(pak))

	//if we couldent find the game, send an error
	if game == nil {
		response = append(
			response,
			Packet{
				Type: 'E', //error packet
				Content: []byte(
					"No joinable game of that name",
				),
			},
		)
	} else {
		//if we could find a game try to join it, and send sucess
		if user.JoinGame(game.Value) {
			response = append(
				response,
				Packet{
					Type: 'J',
					Content: []byte(
						"success",
					),
				},
			)
		} else {
			//otherwise send an error explainign why we couldnt join
			response = append(
				response,
				Packet{
					Type: 'E',
					Content: []byte(
						"could not join game",
					),
				},
			)
		}
	}

	return
}
