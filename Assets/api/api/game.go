package api

type Game struct {
	Players []*Player
	Started bool

	GameSettings
}

type GameSettings struct {
	Password  []byte `json:"password"`
	LobbyName []byte `json:"lobbyname"`
}
