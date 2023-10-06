package layer

import (
	ll "github.com/wdlea/GOGenericLinkedList"
	"github.com/wdlea/flatRaycast/colliders"
	"github.com/wdlea/flatRaycast/point"
)

// Represents a group of colliders
type Layer[ColliderData any] struct {
	Colliders *ll.LinkedList[*colliders.ICollider[ColliderData]]
}

func (l *Layer[ColliderData]) Clear() {
	l.Colliders = new(ll.LinkedList[*colliders.ICollider[ColliderData]])
}

// Returns all hits within the layer
func (l Layer[ColliderData]) GetHits(p point.Point) (hits []ColliderData) {
	hits = make([]ColliderData, 0, 5) //assume that less than 5 of colliders are hit

	for collider := l.Colliders.First; collider != nil; collider = collider.Next {
		col := *collider.Value
		if col.ContainsPoint(p) {
			hits = append(hits, col.GetData())
		}
	}
	return
}

// Gets one collider that hits the point
func (l Layer[ColliderData]) GetHit(p point.Point) (ok bool, hit ColliderData) {
	for collider := l.Colliders.First; collider != nil; collider = collider.Next {
		col := *collider.Value
		if col.ContainsPoint(p) {
			return true, col.GetData()
		}
	}
	return false, *new(ColliderData) //default value
}
