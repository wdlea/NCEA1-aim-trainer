package packets

type ServerBound struct {
	PacketID int `json:"PacketID"`
}

type ClientBound struct {
	IsResponse bool `json:"IsResponse"`
	ResponseID int  `json:"ResponseID"`

	IsFail bool `json:"failed"`
}
