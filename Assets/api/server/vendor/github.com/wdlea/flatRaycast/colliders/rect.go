package colliders

import (
	"github.com/wdlea/flatRaycast/point"
)

// A rectangular collider
type Rect[ColliderData any] struct {
	MinX, MinY, MaxX, MaxY float64
	Data                   ColliderData
}

// Sets the corners of the rect, these corners need to be diagonally opposite
func (r *Rect[_]) SetCorners(c1, c2 point.Point) {
	r.MinX, r.MaxX, r.MinY, r.MaxY = point.Bounds(c1, c2)
}

// Determines whether the point is within the bounds of the rect, excluding edges
func (r Rect[_]) ContainsPoint(p point.Point) bool {

	return p[0] < r.MaxX && p[0] > r.MinX && p[1] < r.MaxY && p[1] > r.MinY //if the rect has 0 width/height this will not register as collision
}

func (r Rect[ColliderData]) GetData() ColliderData {
	return r.Data
}
