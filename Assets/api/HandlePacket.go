package main

import "github.com/wdlea/aimtrainerAPI/api"

func HandlePacket(typeByte byte, marshalledPacket []byte, user *api.Player) (response []packet, doTerminate bool) {
packetTypeSwitch:
	switch typeByte {

	case 'f': //update position
		{
			break packetTypeSwitch
		}
	case 's': //start game(host only)
		{
			break packetTypeSwitch
		}
	case 'j': //join game
		{
			break packetTypeSwitch
		}
	case 'c': //create game
		{
			break packetTypeSwitch
		}
	case 'l': //leave game
		{
			break packetTypeSwitch
		}
	case 't': //terminate connection
		{
			doTerminate = true
			break packetTypeSwitch
		}
	}

	return
}
