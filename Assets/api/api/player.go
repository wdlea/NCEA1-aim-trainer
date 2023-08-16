package api

import "time"

type Player struct {
	Pos  Vector2
	DPos Vector2

	LastUpdate time.Time
}

func (p *Player) ApplyFrame(f Frame) {
	p.Pos = f.Pos
	p.DPos = f.DPos
}

func (p *Player) Tick(deltaTime float64) {
	delta := p.DPos.Copy()
	delta.Scale(deltaTime)

	p.Pos.Add(delta)
}
