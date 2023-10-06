package layer

import (
	"github.com/wdlea/flatRaycast/point"
)

type LayerGroup[ColliderData any] struct {
	Layers []Layer[ColliderData]
}

// Gets all the hits from all the layers
func (l LayerGroup[ColliderData]) GetHits(p point.Point) (hits []ColliderData) {
	hits = make([]ColliderData, 0, 20) //assume less than 20 hits

	for _, layer := range l.Layers {
		hits = append(hits, layer.GetHits(p)...)
	}
	return
}

// Gets the first hit, layers with higher positions on the Layers
// slice are considered "in front" of the lower positions
func (l LayerGroup[ColliderData]) GetHit(p point.Point) (hit ColliderData) {
	var ok bool

	for _, layer := range l.Layers {
		ok, hit = layer.GetHit(p)

		if ok {
			return
		}
	}
	return
}
