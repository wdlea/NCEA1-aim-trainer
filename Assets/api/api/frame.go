package api

import "time"

type Frame struct {
	DPos Vector2 `json:"DPos"`
	Pos  Vector2 `json:"Pos"`

	RecvTime time.Time
}
