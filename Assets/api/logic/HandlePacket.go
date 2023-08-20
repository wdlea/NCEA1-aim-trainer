package logic

import (
	"encoding/json"
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects"
)

func HandlePacket(typeByte byte, marshalledPacket []byte, user *Player) (response []Packet, doTerminate bool) {
	switch typeByte {

	case 'f': //update position
		{
			return HandleFrame(marshalledPacket, user)
		}
	case 'j': //join game
		{
			return HandleJoinGame(marshalledPacket, user)
		}
	case 'c': //create game
		{
			return HandleCreateGame(marshalledPacket, user)
		}
	case 'l': //leave game
		{
			return HandleLeaveGame(marshalledPacket, user)
		}
	case 't': //terminate connection
		{
			doTerminate = true
			return
		}
	case 'n': //set name
		{
			return HandleSetName(marshalledPacket, user)
		}
	default:
		{
			fmt.Printf("Invalid packet type requested: %s, terminating connection\n", string(typeByte))
			doTerminate = true
		}
	}

	return
}

func HandleFrame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	var frame Frame
	err := json.Unmarshal(pak, &frame)
	if err != nil {
		fmt.Printf("Unable to unmarshal message %s from user", string(pak))
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

func HandleCreateGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	resp := HostGameResponse{
		Ok:   user.HostGame(),
		Name: user.Game.Name,
	}
	if !resp.Ok {
		resp.Name = "" //hide name to avoid unintentioanl behavior caused by me being lazy
	}

	marshaledResp, err := json.Marshal(resp)

	if err != nil {
		fmt.Printf("Error while marshaling: %s\n", err)
		return
	}

	response = append(
		response,
		Packet{
			Type:    'C',
			Content: marshaledResp,
		},
	)

	return
}

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

func HandleLeaveGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	user.LeaveGame()
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

const MAX_NAME_LENGTH int = 15

func HandleSetName(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	if user.Game != nil {
		response = append(
			response,
			Packet{
				Type: 'E',
				Content: []byte(
					"You cannot change your name mid game.",
				),
			},
		)
		return
	}

	if len(pak) > 0 && len(pak) < MAX_NAME_LENGTH {
		user.Name = string(pak)

		response = append(
			response,
			Packet{
				Type:    'N',
				Content: pak,
			},
		)
	} else {
		response = append(
			response,
			Packet{
				Type: 'E',
				Content: []byte(
					"Name was either not long enough, too long, or contained banned characters.",
				),
			},
		)
	}

	return
}
