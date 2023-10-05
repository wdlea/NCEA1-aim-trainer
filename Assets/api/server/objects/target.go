package objects

import (
	"encoding/json"
	"fmt"
	"math/rand"

	ll "github.com/wdlea/GOGenericLinkedList" //also, this code was by me too, becuase GOs builtin implemtation was bad
)

type Target struct {
	X, Y, Dx, Dy, DDx, DDy float64

	Scale, Dscale float64

	ID     float64
	llNode *ll.LinkedListNode[*Target]
}

// spawns a target
func SpawnTarget(g *Game) {
	fmt.Println("Spawning target")

	t := new(Target)

	t.X = (rand.Float64() - 0.5) * 200
	t.Y = (rand.Float64() - 0.5) * 200

	t.Dx = (rand.Float64() - 0.5) * 50
	t.Dy = (rand.Float64() - 0.5) * 50

	t.DDx = (rand.Float64() - 0.5) * 25
	t.DDy = (rand.Float64() - 0.5) * 25

	t.Scale = (rand.Float64() * 3) + 0.25
	t.Dscale = ((rand.Float64() - 0.5) / 2.5) //(-0.2)-(0.2)

	t.llNode = g.Targets.AddLast(t)

	t.ID = g.CurrentTargetID
	g.CurrentTargetID++

	marshalled, err := json.Marshal(t)
	if err != nil {
		fmt.Printf("Error while marshalling target: %s\n", err.Error())
		return
	}

	g.SendBroadcastAll(Packet{
		Type:    'T',
		Content: marshalled,
	})
}

// finds a target with a particular ID and destroys it
func (g *Game) DestroyTargetByID(id float64) (ok bool) {
	for current := g.Targets.First; current != nil; current = current.Next { //iterate over linkedlist
		if current.Value.ID == id {
			g.DestroyTarget(current.Value)
			return true
		}
	}

	return false
}

// destroys a target by reference
func (g *Game) DestroyTarget(t *Target) {
	t.llNode.Pop(&g.Targets)

	g.SendBroadcastAll(Packet{
		Type:    'H',
		Content: []byte(fmt.Sprintf("%f", t.ID)),
	})
}
