package main

import (
	"encoding/json"
	"fmt"

	"github.com/wdlea/aimtrainerAPI/objects"
)

func HandlePacket(typeByte byte, marshalledPacket []byte, user *objects.Player) (response []packet, doTerminate bool) {
	switch typeByte {

	case 'f': //update position
		{
			return HandleFrame(marshalledPacket, user)
		}
	case 's': //start game(host only)
		{
			return HandleStartGame(marshalledPacket, user)
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
			fmt.Printf("Invalid packet type requested: %s, terminating connection\n", typeByte)
			doTerminate = true
		}
	}

	return
}

func HandleFrame(pak []byte, user *objects.Player) (response []packet, doTerminate bool) {
	var frame objects.Frame
	err := json.Unmarshal(pak, frame)
	if err != nil {
		fmt.Printf("Unable to unmarshal message %s from user", string(pak))
		return
	}

	user.ApplyFrame(frame)

	marshalledGame, err := json.Marshal(user.Game)
	if err != nil {
		return
	}

	gamePacket := packet{
		Type:    'F',
		Content: marshalledGame,
	}

	response = append(response, gamePacket)

	return
}

func HandleCreateGame(pak []byte, user *objects.Player) (response []packet, doTerminate bool) {

}
func HandleJoinGame(pak []byte, user *objects.Player) (response []packet, doTerminate bool) {

}
func HandleStartGame(pak []byte, user *objects.Player) (response []packet, doTerminate bool) {

}
func HandleLeaveGame(pak []byte, user *objects.Player) (response []packet, doTerminate bool) {
	user.LeaveGame()
}
