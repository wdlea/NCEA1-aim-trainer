package logic

import (
	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

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
