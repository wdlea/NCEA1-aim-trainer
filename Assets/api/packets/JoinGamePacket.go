package packets

type JoinGameRequest struct {
	ServerBound

	LobbyName string `json:"LobbyName"`
	Password  string `json:"password"`
}

const (
	JOIN_GAME_SUCCESS int = iota
	JOIN_GAME_LOBBY_FULL
	JOIN_GAME_ALREADY_STARTED
	JOIN_GAME_INCORRECT_PASSWORD
	JOIN_GAME_NOT_FOUND
	JOIN_GAME_ALREADY_IN_GAME
	JOIN_GAME_UNKNOWN_ERROR
)

type JoinGameResponse struct {
	ClientBound
	Status int `json:"status"`
}
