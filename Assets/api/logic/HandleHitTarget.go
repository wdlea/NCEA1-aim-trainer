package logic

import (
	"fmt"
	"strconv"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// Handles the 'h' packet, which is sent whenever the client hits a target
func HandleHitTarget(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	targetID, err := strconv.Atoi(string(pak))

	fmt.Printf("%s tried hit target, ", user.Name)

	if err != nil {
		fmt.Println("but failed miserably to encode.")
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
		fmt.Println("but missed.")
		returnValue = 0
		fmt.Println("Target was attempted to be destroyed that was not present in game") //dont error out, but log it becuase that could help me deubug
	} else {
		//broadcast that the target is now hit
		fmt.Println("and hit.")
	}

	user.Score += float64(returnValue)

	response = append(response,
		Packet{
			Type:    'H',
			Content: []byte{returnValue},
		},
	)

	return
}
