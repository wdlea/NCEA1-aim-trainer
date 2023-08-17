package main

import (
	"encoding/json"
	"fmt"
	"net"
	"time"

	"github.com/wdlea/aimtrainerAPI/api"
	"github.com/wdlea/aimtrainerAPI/packets"
)

func HandleConn(conn net.Conn) {
	defer conn.Close()

	fmt.Printf("New player connection from %s \n", conn.RemoteAddr().String())

	user := new(api.Player)

	for {
		buf := make([]byte, 2048)

		n, err := conn.Read(buf)
		if err != nil {
			fmt.Printf("Error occurred on packet recieve: %s from connection %s\n", err.Error(), conn.RemoteAddr().String())
			break
		}
		if n <= 0 {
			fmt.Printf("No data recieved from connection when packet was sent from %s\n", conn.RemoteAddr().String())
			break
		}

		buf = buf[:n]

		fmt.Printf("Packet %s(length %d) recieved from %s\n", string(buf), n, conn.RemoteAddr().String())

		marshalledPacket := buf[1:]

		doResp, resp := HandlePacket(buf[0], marshalledPacket, user)
		if doResp {
			_, err := conn.Write(resp)
			if err != nil {
				fmt.Printf("Error when responding to client: %s \n", err.Error())
			}
		}
	}
}

func HandlePacket(typeByte byte, marshalledPacket []byte, user *api.Player) (doResponse bool, response []byte) {
packetTypeSwitch:
	switch typeByte {
	case 'f': //update position
		{
			var f api.Frame
			err := json.Unmarshal(marshalledPacket, &f)

			if err != nil {
				break packetTypeSwitch
			}
			f.RecvTime = time.Now()

			user.ApplyFrame(f)

			break packetTypeSwitch
		}
	case 's': //start game(host only)
		{
			break packetTypeSwitch
		}
	case 'j': //join game
		{
			var req packets.JoinGameRequest
			err := json.Unmarshal(marshalledPacket, &req)

			resp := new(packets.JoinGameResponse)

			if err != nil {
				resp.IsFail = true
			} else if user.CurrentGame != nil {
				resp.Status = packets.JOIN_GAME_ALREADY_IN_GAME
			} else {
				game := GetGame(req.LobbyName)
				if game == nil {
					resp.Status = packets.JOIN_GAME_NOT_FOUND
				} else {

					success, incorrectp, full, started := user.JoinGame(game, req.Password)

					if success {
						resp.Status = packets.JOIN_GAME_SUCCESS
						user.CurrentGame = game
					} else if started {
						resp.Status = packets.JOIN_GAME_ALREADY_STARTED
					} else if incorrectp {
						resp.Status = packets.JOIN_GAME_INCORRECT_PASSWORD
					} else if full {
						resp.Status = packets.JOIN_GAME_LOBBY_FULL
					} else {
						resp.Status = packets.JOIN_GAME_UNKNOWN_ERROR

						fmt.Println("Unknown unsucess when joining game")
					}
				}

			}

			var mar []byte
			err = nil
			mar, err = json.Marshal(resp)

			response = append([]byte("j"), mar...)
			doResponse = err == nil

			if !doResponse {
				fmt.Printf("Error while marshalling packet: %s", err.Error())
			}
		}
	case 'c': //create game
		{
			break packetTypeSwitch
		}
	case 'l': //leave game
		{
			break packetTypeSwitch
		}
	}

	return
}
