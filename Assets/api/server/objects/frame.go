package objects

// represents an instant of the players motion
type Frame struct {
	X, Y, Dx, Dy, DDx, DDy float64
}

// apply a frame to a player
func (p *Player) ApplyFrame(f Frame) {

	//add the frame
	p.X, p.Y, p.Dx, p.Dy, p.DDx, p.DDy = f.X, f.Y, f.Dx, f.Dy, f.DDx, f.DDy
}