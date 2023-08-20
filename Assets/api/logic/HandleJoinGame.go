package logic

import (
	"encoding/json"
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

func HandleJoinGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	var joinReq JoinGameRequest

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

	err := json.Unmarshal(pak, &joinReq)
	if err != nil {
		fmt.Printf("Error: %s while unmarshaling %s", err.Error(), string(pak))
		return
	}

	game := FindGame(joinReq.Name)

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
