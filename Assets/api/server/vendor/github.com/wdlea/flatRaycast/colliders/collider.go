package colliders

import "github.com/wdlea/flatRaycast/point"

// Represents a collider
type ICollider[ColliderData any] interface {
	ContainsPoint(point.Point) bool
	GetData() ColliderData
}
