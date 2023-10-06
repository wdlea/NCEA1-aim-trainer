package objects

import "time"

const GAME_TICK_RATE = 40 //hz
const GAME_TICK_INTERVAL = time.Second / GAME_TICK_RATE
const GAME_TICK_INTERVAL_SECONDS = 1.0 / GAME_TICK_RATE

const TARGET_SPAWN_RATE = 2 //hz

const COUNTDOWN_DURATION = 1200 * time.Millisecond //fast countdown, becuase that looks cooler

const GAME_DURATION = 5 * time.Minute

const GAME_NAME_LENGTH = 8

const PLAYER_VELOCITY_DAMPING_FACTOR = 0.5
const PLAYER_ACCELLERATION_DAMPING_FACTOR = 0.1

const BROADCAST_PREFIX = 'B'