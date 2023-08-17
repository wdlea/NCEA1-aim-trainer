package objects

type Player struct {
	Name string `json:"name"`

	Game *Game

	X, Y, Dx, Dy float32
}

type Frame struct {
	X, Y, Dx, Dy float32
}

func (p *Player) HostGame() {

}

func (p *Player) JoinGame() {

}

func (p *Player) LeaveGame() {
	if p.Game != nil {
		p.Game.RemovePlayer(p)
	}
	p.Game = nil
}

func (p *Player) ApplyFrame(f Frame) {
	p.X, p.Y, p.Dx, p.Dy = f.X, f.Y, f.Dx, f.Dy
}

func (p *Player) Update(deltaTime float32) {
	p.X += p.Dx * deltaTime
	p.Y += p.Dy * deltaTime
}

func (p *Player) Dispose() {
	p.LeaveGame()
}
