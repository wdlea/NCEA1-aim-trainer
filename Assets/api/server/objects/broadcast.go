package objects

import "fmt"

// sends a broadcast to the player
func (p *Player) SendBroadcast(message Packet) {
	p.Conn.Write([]byte(fmt.Sprintf(
		string(BROADCAST_PREFIX)+"%s%s\n",
		string(message.Type),
		message.Content,
	)))
}

// sends a broadcast to all players
func (g *Game) SendBroadcastAll(p Packet) {
	for _, player := range g.Players {
		player.SendBroadcast(p)
	}
}
