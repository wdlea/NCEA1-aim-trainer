package logic

import (
	"encoding/json"
	"fmt"

	. "github.com/wdlea/aimtrainerAPI/objects" //I like . imports
)

func HandleCreateGame(pak []byte, user *Player) (response []Packet, doTerminate bool) {
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
	}

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
