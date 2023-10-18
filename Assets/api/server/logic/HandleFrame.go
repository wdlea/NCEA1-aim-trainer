package logic

import (
	"encoding/json"
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// Handles the 'f' packet, which is whenever the client wishes to update their position and velocity with the server
func HandleFrame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	fmt.Printf("Handling Frame from %s\n", user.Name)

	//create the frame object and unmarshal the JSON into it
	var frame Frame
	err := json.Unmarshal(pak, &frame)

	//if there was an error just return, dont send an error packet, the
	if err != nil {
		fmt.Printf("Unable to unmarshal message %s from user %s with error %s\n", string(pak), string(user.Name), err.Error())
		return
	}

	//if the user is in a game
	if user.Game != nil {

		//and it is running
		if user.Game.State == GAME_RUNNING {

			//apply the frame
			user.ApplyFrame(frame)

			//marshal the game for the respones
			marshalledGame, err := json.Marshal(user.Game)
			if err != nil {
				fmt.Printf("Error while marshaling game: %s\n", err.Error())
				return
			}

			//send it back to the user
			response = append(
				response,
				Packet{
					Type:    'F',
					Content: marshalledGame,
				},
			)
		} else {
			//send error packet
			response = append(
				response,
				Packet{
					Type: 'E', //error packet
					Content: []byte(
						"Game not started yet",
					),
				},
			)
		}

	} else {
		//also send error packet
		response = append(
			response,
			Packet{
				Type: 'E', //error packet
				Content: []byte(
					"User not in game",
				),
			},
		)
	}

	return
}
