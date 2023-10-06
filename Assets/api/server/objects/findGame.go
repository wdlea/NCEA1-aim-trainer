package objects

import (
	"fmt"

	ll "github.com/wdlea/GOGenericLinkedList"
)

var ActiveGames *ll.LinkedList[*Game]

// Searches for a game by the name
func FindGame(name string) *ll.LinkedListNode[*Game] {
	//first check if the name is valid
	if len(name) == GAME_NAME_LENGTH {

		//if it is, try a game with it and return it
		for game := ActiveGames.First; game != nil; game = game.Next {
			if game.Value.Name == name {
				return game
			}
		}
	} else {
		//otherwise print an error to console and return nil
		fmt.Println("Name with invalid length supplied, skipping")
	}

	return nil
}