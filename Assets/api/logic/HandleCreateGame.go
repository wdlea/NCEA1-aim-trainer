package logic

import (
	"encoding/json"
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

// Handles a player creating a game
func HandleCreateGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
	//if the player does not have a name send an error
	if len(user.Name) <= 0 {
		response = append(
			response,
			Packet{
				Type: 'E',
				Content: []byte(
					"Cannot create a game without having a name.",
				),
			},
		)
		return
	}

	//create a game and send a rasponse with the code for others to join
	resp := HostGameResponse{
		Ok:   user.HostGame(),
		Name: user.Game.Name,
	}

	//if we couldent host a game for whatever reason, hide the name to prevent errors
	if !resp.Ok {
		resp.Name = "" //hide name to avoid unintentioanl behavior caused by me being lazy
	}

	//marshal the response to send back to the user
	marshaledResp, err := json.Marshal(resp)

	//if there was an error dont send anything and print the error to logs
	if err != nil {
		fmt.Printf("Error while marshaling: %s\n", err)
		return
	}

	//send the response
	response = append(
		response,
		Packet{
			Type:    'C',
			Content: marshaledResp,
		},
	)

	return
}
