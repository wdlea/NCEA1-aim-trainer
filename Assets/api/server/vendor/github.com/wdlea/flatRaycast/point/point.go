package point

import "math"

type Point [2]float64

// Subtracts p2 from p1
func Sub(p1 Point, p2 Point) (result Point) {
	return Point{
		p1[0] - p2[0],
		p1[1] - p2[1],
	}
}

// Adds p1 and p2
func Add(p1 Point, p2 Point) (result Point) {
	return Point{
		p1[0] + p2[0],
		p1[1] + p2[1],
	}
}

// Calculates the magnitude of the point as if it were a vector
func (p Point) Mag() float64 {
	return math.Sqrt(p.SqrMag())
}

// Calculates the squared magnitude of the point to avoid math.Sqrt call
func (p Point) SqrMag() float64 {
	return (p[0]*p[0] + p[1]*p[1])
}
