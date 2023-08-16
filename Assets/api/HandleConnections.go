package main

import (
	"encoding/json"
	"fmt"
	"net"
	"time"

	"github.com/wdlea/aimtrainerAPI/api"
)

func HandleConn(conn net.Conn) {
	defer conn.Close()

	fmt.Printf("New player connection from %s \n", conn.RemoteAddr().String())

	user := new(api.Player)

	for {
		buf := make([]byte, 2048)

		n, err := conn.Read(buf)
		if err != nil {
			fmt.Printf("Error occurred on packet recieve: %s from connection %s", err.Error(), conn.RemoteAddr().String())
			break
		}
		if n <= 0 {
			fmt.Printf("No data recieved from connection when packet was sent from %s", conn.RemoteAddr().String())
			break
		}

		buf = buf[:n]

		fmt.Printf("Packet %s(length %d) recieved from %s", string(buf), n, conn.RemoteAddr().String())

	packetTypeSwitch:
		switch buf[0] {
		case 'f':
			{
				var f api.Frame
				err := json.Unmarshal(buf[1:], &f)

				if err != nil {
					break packetTypeSwitch
				}
				f.RecvTime = time.Now()

				user.ApplyFrame(f)

				break packetTypeSwitch
			}
		case 's':
			{
				//start game(host only)
				break packetTypeSwitch
			}
		case 'j':
			{
				//join game
				break packetTypeSwitch
			}
		case 'c':
			{
				//create game
				break packetTypeSwitch
			}
		case 'l':
			{
				//leave game
				break packetTypeSwitch
			}
		}
	}
}
