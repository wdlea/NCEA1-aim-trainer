package objects

import (
	"encoding/json"
	"fmt"

	ll "github.com/wdlea/GOGenericLinkedList" //also, this code was by me too, becuase GOs builtin implemtation was bad
)

type Target struct {
	X, Y, Dx, Dy float64

	ID     float64
	llNode *ll.LinkedListNode[*Target]
}

func SpawnTarget(g *Game) {
	t := new(Target)

	//todo randomise position and velocity

	t.llNode = g.Targets.AddLast(t)

	t.ID = g.CurrentTargetID
	g.CurrentTargetID++

	marshalled, err := json.Marshal(t)
	if err != nil {
		fmt.Printf("Error while marshalling target: %s\n", err.Error())
		return
	}

	g.SendBroadcast(Packet{
		Type:    'T',
		Content: marshalled,
	})
}

func (g *Game) DestroyTargetByID(id float64) {
	for current := g.Targets.First; current != nil; current = current.Next { //iterate over linkedlist
		if current.Value.ID == id {
			g.DestroyTarget(current.Value)
			return
		}
	}
}

func (g *Game) DestroyTarget(t *Target) {
	t.llNode.Pop(&g.Targets)

	g.SendBroadcast(Packet{
		Type:    'D',
		Content: []byte(fmt.Sprintf("%f", t.ID)),
	})
}
