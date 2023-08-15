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
