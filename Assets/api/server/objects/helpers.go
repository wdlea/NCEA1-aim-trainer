package objects

import "math/rand"

const SAFE_CHARS = "qwertyuiopasdfghjklzxcvbnm"

//generates a random URL-safe string by picking chars from SAFE_CHARS
func RandomURLSafeString(length int) string {
	s := ""

	//iterate length times
	for i := 0; i < length; i++ {

		//generate a random number within the range of the chars
		num := rand.Int()
		num = num % len(SAFE_CHARS)

		//add the char at that index to the string
		s += string(SAFE_CHARS[num])
	}

	//return the string
	return s
}
