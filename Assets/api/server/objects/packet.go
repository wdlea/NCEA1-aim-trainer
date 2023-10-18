package objects

// packets with lowercase types are serverbound, uppercase types are client bound
type Packet struct {
	Type    byte
	Content []byte
}

//create byte representation of packet
func (p Packet) ToBytes(suffix byte) []byte {

	bytes := append([]byte{p.Type}, p.Content...)
	bytes = append(bytes, suffix)

	return bytes
}
