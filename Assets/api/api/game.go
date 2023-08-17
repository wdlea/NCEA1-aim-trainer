package api

import "time"

const tick_frequency = 40                          //hz
const tick_interval = time.Second / tick_frequency //some unit of time, use tick_interval_seconds if you want the interval in a specific unit
const tick_interval_seconds = 1.0 / tick_frequency //s

const game_duration_minutes = 2                           //m
const game_duration = time.Minute * game_duration_minutes //some unit of time

const (
	GAME_STATE_PENDING_PLAYERS = iota
	GAME_STATE_PENDING_START
	GAME_STATE_RUNNING
	GAME_STATE_PENDING_DELETION
)

type Game struct {
	Players []*Player
	State   int

	Owner *Player

	ticker *time.Ticker

	done chan int

	GameSettings
}

func (g *Game) Start() {
	g.ticker = time.NewTicker(tick_interval)
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
		for {
			select {
			case <-g.ticker.C:
				{
					g.Update(float64(tick_interval_seconds))
				}
			case <-g.done:
				{
					return
				}
			}
		}

	}(g)
}

func (g *Game) Update(deltatime float64) {
	for _, p := range g.Players {
		if p == nil {
			g.State = GAME_STATE_PENDING_DELETION
			g.done <- 0
		}
		p.Tick(deltatime)
	}
}

type GameSettings struct {
	Password  string `json:"password"`
	LobbyName string `json:"lobbyname"`
}
