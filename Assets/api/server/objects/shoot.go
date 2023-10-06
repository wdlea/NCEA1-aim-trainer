package objects

import (
	"math/rand"

	"github.com/wdlea/flatRaycast/colliders"
	"github.com/wdlea/flatRaycast/layer"
	"github.com/wdlea/flatRaycast/point"
)

type colliderData struct {
	IsTarget bool
	ID       float64
}

const BLOCK_LAYER_INDEX = 0
const TARGET_LAYER_INDEX = 1

func (g *Game) InitCollisions() {
	g.Layers = new(layer.LayerGroup[colliderData])

	blockLayer := new(layer.Layer[colliderData])
	targetLayer := new(layer.Layer[colliderData])
	blockLayer.Clear()
	targetLayer.Clear()

	g.Layers.Layers = append(g.Layers.Layers, blockLayer, targetLayer)
}

func (p *Player) Shoot(shootPoint point.Point) (wasHit bool, hitID int) {
	p.Game.DestructionLock.Lock()
	defer p.Game.DestructionLock.Unlock()

	//regenerate the targets(they move, yay!)
	p.Game.Layers.Layers[TARGET_LAYER_INDEX].Clear()
	for target := p.Game.Targets.First; target != nil; target = target.Next {
		col := colliders.Circle[colliderData]{
			Centre: point.Point{
				target.Value.X,
				target.Value.Y,
			},
			Radius: target.Value.Scale,
		}

		//cast to interface
		inter := colliders.ICollider[colliderData](col)

		p.Game.Layers.Layers[TARGET_LAYER_INDEX].Colliders.AddLast(&inter)
	}

	//Get the hit
	data := p.Game.Layers.GetHit(shootPoint)

	if data.IsTarget {
		return data.IsTarget, data.ID
	}
	return data.IsTarget, -1 //I want a runtime error if I am using this improperly in both server and client, an array index of -1 does this
}

type Block struct {
	C1, C2 point.Point
	ID     float64
}

func (g *Game) AddBlockRandomly() {
	collidersList := g.Layers.Layers[BLOCK_LAYER_INDEX].Colliders

	corner1, corner2 := point.RandomPointWithinRect(GAME_MIN_POINT, GAME_MAX_POINT), point.RandomPointWithinRect(GAME_MIN_POINT, GAME_MAX_POINT)

	collider := new(colliders.Rect[colliderData])
	collider.SetCorners(corner1, corner2)

	//cast to interface
	inter := colliders.ICollider[colliderData](collider)

	collidersList.AddLast(&inter)

	block := Block{
		ID: g.CurrentBlockID,
		C1: corner1,
		C2: corner2,
	}
	g.CurrentBlockID++

	//send add broadcast

	_ = block
}

type ChanceSelector[ColliderData any] struct {
	Chance float32
}

func (c ChanceSelector[ColliderData]) IsValid(collider colliders.ICollider[ColliderData]) bool {
	return rand.Float32() < c.Chance
}

// Removes *at most* one block from the scene
func (g *Game) DeleteBlockRandomly() {
	objects := g.Layers.Layers[BLOCK_LAYER_INDEX].RemoveObjectsBySelector(ChanceSelector[colliderData]{Chance: 0.5}, 1)

	for _, obj := range objects {
		//send destroy broadcast
		_ = obj
	}
}
