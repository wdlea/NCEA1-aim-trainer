package logic

import (
	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

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
