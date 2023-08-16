package api_test

import "testing"

func Assert(t *testing.T, condition bool, errorMsg string) {
	if !condition {
		t.Fatalf(errorMsg)
	}
}

func AssertErrorFree(t *testing.T, err error) {
	if err != nil {
		t.Fatalf(err.Error())
	}
}
