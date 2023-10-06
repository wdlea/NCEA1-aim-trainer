package colliders

import "github.com/wdlea/flatRaycast/point"

// A circular collider
type Circle[ColliderData any] struct {
	Centre point.Point
	Radius float64
	Data   ColliderData
}

// Determines whether the point is within the circle, excluding edges
func (c Circle[_]) ContainsPoint(p point.Point) bool {
	return c.Radius*c.Radius > point.Sub(p, c.Centre).SqrMag() //< because if it is on the edge it doesnt count
}

func (c Circle[ColliderData]) GetData() ColliderData {
	return c.Data
}
