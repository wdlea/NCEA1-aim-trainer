package point

import (
	"math"
	"math/rand"
)

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

// Returns a random point that lies within a rect defined by 2 points
func RandomPointWithinRect(p1, p2 Point) Point {
	//M*i*nX, M*a*xX, M*i*nY, M*a*xY, shortened
	iX, aX, iY, aY := Bounds(p1, p2)

	//calculate ranges
	dX, dY := aX-iX, aY-iY

	//calculate point in range 0, d?, then add i? to make it between i? and a?, ? -> X or Y
	return Point{
		rand.Float64()*dX + iX,
		rand.Float64()*dY + iY,
	}
}

// Calculates the lower and upper bounds of 2 points
func Bounds(p1, p2 Point) (minX, maxX, minY, maxY float64) {
	return math.Min(p1[0], p2[0]), math.Max(p1[0], p2[0]), math.Min(p1[1], p2[1]), math.Max(p1[1], p2[1])
}
