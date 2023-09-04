package objects

type Target struct {
	X, Y, Dx, Dy float64

	ID string //Using a float64(8 bytes) is a bit overkill, so I will use a random string(~4 chars -> 4 bytes) instead
}

func SpawnTarget(g *Game) {
	t := new(Target)

	//todo randomise position and velocity

	g.Targets = append(g.Targets, *t)

}
