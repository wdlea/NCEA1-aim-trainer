package api

import "time"

const tick_frequency = 20                          //hz
const tick_interval = time.Second / tick_frequency //some unit of time
const tick_interval_seconds = 1 / tick_frequency   //s

const game_duration_minutes = 2                           //m
const game_duration = time.Minute * game_duration_minutes //some unit of time

type Game struct {
	Players []*Player
	Started bool

	Owner *Player

	ticker *time.Ticker

	done chan int

	GameSettings
}

func (g *Game) Start() {
	g.ticker = time.NewTicker(tick_frequency)
	timer := time.NewTimer(game_duration)

	go func(g *Game, timer *time.Timer) {
		select {
		case <-timer.C:
			{
				g.done <- 0
			}
		case <-g.done:
			{
				timer.Stop()
			}
		}
		g.ticker.Stop()

	}(g, timer)

	go func(g *Game) {
		select {
		case <-g.ticker.C:
			{
				g.Update(float64(tick_interval_seconds))
			}
		}
	}(g)
}

func (g *Game) Update(deltatime float64) {
	for _, p := range g.Players {
		if p == nil {
			g.Started = true
		}
		p.Tick(deltatime)
	}
}

type GameSettings struct {
	Password  string `json:"password"`
	LobbyName string `json:"lobbyname"`
}
