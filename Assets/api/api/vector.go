package api

// A vector2 is a 2D vector
type Vector2 [2]float64

// Adds v2 to v1, changes v1 to the answer
func (v1 *Vector2) Add(v2 Vector2) {
	v1[0] += v2[0]
	v1[1] += v2[1]
}

func (v *Vector2) Scale(factor float64) {
	v[0] *= factor
	v[1] *= factor
}

func (v *Vector2) Copy() Vector2 {
	return *v
}
