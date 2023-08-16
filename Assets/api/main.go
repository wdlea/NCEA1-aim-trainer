package main

import (
	"context"
	"fmt"
	"net"
	"os"

	"github.com/wdlea/aimtrainerAPI/api"
)

var games []*api.Game

func main() {
	games = make([]*api.Game, 0, 32)

	listener, err := net.Listen("tcp", ":8088")
	if err != nil {
		panic("Server failed to start: " + err.Error())
	}
	defer listener.Close()

	serverContext, stopServer := context.WithCancel(context.TODO())

	defer stopServer()

	go AcceptConnections(listener, serverContext)

	fmt.Println("Press enter to stop server")

	for n, err := fmt.Scanln(); err != nil || n == 0; {
	}

	fmt.Println("Stopping...")

	os.Exit(0)
}

func AcceptConnections(listener net.Listener, ctx context.Context) {
	connChan := make(chan net.Conn)
	go AcceptAllConnections(listener, connChan)

AcceptLoop:
	for {
		select {
		case conn := <-connChan:
			{
				go HandleConn(conn)
			}
		case <-ctx.Done():
			{
				break AcceptLoop
			}
		}
	}

}

func AcceptAllConnections(listener net.Listener, connChannel chan net.Conn) {
	for {
		conn, err := listener.Accept()

		if err != nil {
			fmt.Printf("Error in accepting connection: " + err.Error())
			continue
		}
		connChannel <- conn
	}
}
