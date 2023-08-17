package objects

import "math/rand"

const safeChars = "qwertyuiopasdfghjklzxcvbnm"

func RandomURLSafeString(length int) string {
	s := ""

	for i := 0; i < length; i++ {
		num := rand.Int()
		num = num % len(safeChars)

		s += string(safeChars[num])
	}

	return s
}
