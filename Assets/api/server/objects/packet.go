package objects

// packets with lowercase types are serverbound, uppercase types are client bound
type Packet struct {
	Type    byte
	Content []byte
}
