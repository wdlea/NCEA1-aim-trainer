package logic

import (
	"encoding/json"
	"fmt"

	"github.com/wdlea/aimtrainerAPI/objects"
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
	default:
		{
			fmt.Printf("Invalid packet type requested: %s, terminating connection\n", string(typeByte))
			doTerminate = true
		}
	}

	return
}

func HandleFrame(pak []byte, user *objects.Player) (response []Packet, doTerminate bool) {
	var frame objects.Frame
	err := json.Unmarshal(pak, &frame)
	if err != nil {
		fmt.Printf("Unable to unmarshal message %s from user", string(pak))
		return
	}

	user.ApplyFrame(frame)

	marshalledGame, err := json.Marshal(&user.Game)
	if err != nil {
		return
	}

	gamePacket := Packet{
		Type:    'F',
		Content: marshalledGame,
	}

	response = append(response, gamePacket)

	return
}

func HandleCreateGame(pak []byte, user *objects.Player) (response []Packet, doTerminate bool) {
	user.HostGame()
	return
}

func HandleJoinGame(pak []byte, user *objects.Player) (response []Packet, doTerminate bool) {
	var joinReq objects.JoinGameRequest

	err := json.Unmarshal(pak, &joinReq)
	if err != nil {
		fmt.Printf("Error: %s while unmarshaling %s", err.Error(), string(pak))
	}

	return
}

func HandleLeaveGame(pak []byte, user *objects.Player) (response []Packet, doTerminate bool) {
	user.LeaveGame()
	return
}
