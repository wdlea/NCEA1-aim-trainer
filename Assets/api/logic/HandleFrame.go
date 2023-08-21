package logic

import (
	"encoding/json"
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

func HandleFrame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	var frame Frame
	err := json.Unmarshal(pak, &frame)
	if err != nil {
		fmt.Printf("Unable to unmarshal message %s from user %s with error %s\n", string(pak), string(user.Name), err.Error())
		return
	}

	if user.Game != nil {

		if user.Game.State == GAME_RUNNING {
			user.ApplyFrame(frame)

			marshalledGame, err := json.Marshal(&user.Game)
			if err != nil {
				fmt.Printf("Error while marshaling game: %s\n", err.Error())
				return
			}

			response = append(
				response,
				Packet{
					Type:    'F',
					Content: marshalledGame,
				},
			)
		} else {
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
