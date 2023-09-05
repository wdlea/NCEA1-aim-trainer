package logic

import (
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// handles routing recieved packets based on the first byte
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
	case 'h': //hit a target
		{
			return HandleHitTarget(marshalledPacket, user)
		}
	default:
		{
			fmt.Printf("Invalid packet type requested: %s, terminating connection\n", string(typeByte))
			doTerminate = true
		}
	}

	return
}
