package objects

import (
	"math"
	"math/rand"
)

// updates a game, should be called based on the server tick rate
func (g *Game) Update(deltatime float64) {
	for _, player := range g.Players {
		player.Update(deltatime)
	}

	//roll chance, proportional to deltaTime so that the spawn chance is uniform
	if rand.Float64() < TARGET_SPAWN_RATE*deltatime {
		SpawnTarget(g)
	}
}

// Updates a player based on theyr dX and dY
func (p *Player) Update(deltaTime float64) {
	DFactor := math.Pow(PLAYER_VELOCITY_DAMPING_FACTOR, deltaTime)

	p.Dx *= DFactor
	p.Dy *= DFactor

	DDFactor := math.Pow(PLAYER_ACCELLERATION_DAMPING_FACTOR, deltaTime)

	p.DDx *= DDFactor
	p.DDy *= DDFactor

	p.Dx += p.DDx * deltaTime
	p.Dy += p.DDy * deltaTime

	p.X += p.Dx * deltaTime
	p.Y += p.Dy * deltaTime
}