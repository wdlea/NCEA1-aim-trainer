package api

// A vector2 is a 2D vector
type Vector2 [2]int

// Adds v2 to v1, changes v1 to the answer
func (v1 *Vector2) Add(v2 Vector2) {
	v1[0] += v2[0]
	v1[1] += v2[1]
}
