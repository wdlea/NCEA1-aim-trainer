package logic

import (
	"fmt"
	"strconv"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

func HandleHitTarget(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	targetID, err := strconv.Atoi(string(pak))

	if err != nil {
		response = append(response,
			Packet{
				Type: 'E',
				Content: []byte(
					"Failed to read target ID from packet",
				),
			},
		)
		return
	}

	hit := !user.Game.DestroyTargetByID(float64(targetID))

	returnValue := byte(1)

	if !hit {
		returnValue = 0
		fmt.Println("Target was attempted to be destroyed that was not present in game") //dont error out, but log it becuase that could help me deubg
	}

	response = append(response,
		Packet{
			Type:    'H',
			Content: []byte{returnValue},
		},
	)

	return
}
