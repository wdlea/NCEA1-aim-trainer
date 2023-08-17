package main

import (
	"fmt"
	"net"

	"github.com/wdlea/aimtrainerAPI/objects"
)

const BUFFER_SIZE = 1024
const PACKET_SEPERATOR = '\n'

// packets with lowercase types are serverbound, uppercase types are client bound
type packet struct {
	Type    byte
	Content []byte
}

func HandleConn(conn net.Conn) {
	defer conn.Close()

	fmt.Printf("New player connection from %s \n", conn.RemoteAddr().String())

	user := new(objects.Player)
	defer user.Dispose()

	inboundDataChan := make(chan byte, BUFFER_SIZE)
	inboundPacketChan := make(chan packet, 8)

	outboundPacketChan := make(chan packet, 8)

	go HandleRecieve(inboundDataChan, conn)
	go HandleBytes(inboundDataChan, inboundPacketChan)
	go HandlePackets(inboundPacketChan, outboundPacketChan, user)
	HandleSend(outboundPacketChan, conn) //block until finished, if one of the above goroutines errors out, it will "cascade" down to this one
}

func HandleRecieve(dataChan chan<- byte, conn net.Conn) {
	defer close(dataChan)

	for {
		buf := make([]byte, BUFFER_SIZE)
		n, err := conn.Read(buf)

		if err == net.ErrClosed {
			return
		} else if err != nil {
			fmt.Printf("Error while recieving fron conn: %s \n", err.Error())
		}

		buf = buf[:n]

		for _, b := range buf {
			dataChan <- b
		}
	}
}

func HandleBytes(dataChan <-chan byte, packetChan chan<- packet) {
	defer close(packetChan)

	for {
		encodedPacket := make([]byte, 0, 256)

		typeByte, open := <-dataChan

		if !open {
			return
		}

	packetLoop:
		for {
			currentByte := <-dataChan

			if currentByte == PACKET_SEPERATOR {
				break packetLoop
			} else {
				encodedPacket = append(encodedPacket, currentByte)
			}
		}

		packetChan <- packet{
			Type:    typeByte,
			Content: encodedPacket,
		}
	}
}

func HandlePackets(inbound <-chan packet, outbound chan<- packet, user *objects.Player) {
	defer close(outbound)

	for {
		pak, open := <-inbound
		if !open {
			return
		}

		resps, terminate := HandlePacket(pak.Type, pak.Content, user)

		if terminate {
			fmt.Println("Connection terminated by client through intentional behaviour") //connection lost, but cleanly
			return
		}

		for _, resp := range resps {
			outbound <- resp
		}
	}
}

func HandleSend(outbound <-chan packet, conn net.Conn) {
	for {
		currentSend, open := <-outbound

		if !open {
			return
		}

		bytesToSend := append([]byte{currentSend.Type}, currentSend.Content...)
		bytesToSend = append(bytesToSend, PACKET_SEPERATOR)

		_, err := conn.Write(bytesToSend)

		if err != nil {
			fmt.Printf("Error in writing to connection: %s\n", err.Error())
			return
		}
	}
}
