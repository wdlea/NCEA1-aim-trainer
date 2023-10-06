package objects

import (
	"github.com/wdlea/flatRaycast/colliders"
	"github.com/wdlea/flatRaycast/layer"
	"github.com/wdlea/flatRaycast/point"
)

type colliderData struct {
	IsTarget bool
	ID       int
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

func (p *Player) Shoot(point point.Point) (wasHit bool, hitID int) {
	p.Game.DestructionLock.Lock()
	defer p.Game.DestructionLock.Unlock()

	data := p.Game.Layers.GetHit(point)

	if data.IsTarget {
		return data.IsTarget, data.ID
	}
	return data.IsTarget, -1 //I want a runtime error if I am using this improperly in both server and client, an array index of -1 does this
}

type Block struct {
	C1, C2 point.Point
	ID     int
}

func (g *Game) AddBlockRandomly() {
	collidersList := g.Layers.Layers[BLOCK_LAYER_INDEX].Colliders

	corner1, corner2 := point.RandomPointWithinRect(GAME_MIN_POINT, GAME_MAX_POINT), point.RandomPointWithinRect(GAME_MIN_POINT, GAME_MAX_POINT)

	collider := new(colliders.Rect[colliderData])
	collider.SetCorners(corner1, corner2)

	//cast to interface
	inter := colliders.ICollider[colliderData](collider)

	collidersList.AddLast(&inter)
}

func (g *Game) DeleteBlockRandomly() {

}

func (g *Game) addTargetToSimulation(t Target) {

}
