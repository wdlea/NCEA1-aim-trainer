package api_test

import (
	"encoding/json"
	"testing"

	"github.com/wdlea/aimtrainerAPI/api"
)

func TestMarshal(t *testing.T) {
	v1 := api.Vector2{0, 1}

	marshaled, err := json.Marshal(v1)
	if err != nil {
		t.Fatalf("Could not marshal vector")
	}

	var v2 api.Vector2

	err = json.Unmarshal(marshaled, &v2)
	if err != nil {
		t.Fatalf("Unable to remarshal vector from %s", string(marshaled))
	}
}

func TestCopy(t *testing.T) {
	v1 := api.Vector2{0, 1}
	v2 := v1.Copy()
	v1[0] = 7
	Assert(
		t,
		v1[0] != v2[0],
		"Copy returned a reference, not a copy",
	)
}

func TestScale(t *testing.T) {
	v1 := api.Vector2{0, 1}

	v1.Scale(-1)
	Assert(
		t,
		v1[0] == 0 && v1[1] == -1,
		"Scaled incorretly(fac -1)",
	)

	v1.Scale(7)
	Assert(
		t,
		v1[0] == 0 && v1[1] == -7,
		"Scaled incorretly(fac 7)",
	)
	v1.Scale(0)
	Assert(
		t,
		v1[0] == 0 && v1[1] == 0,
		"Scaled incorretly(fac 0)",
	)
}
