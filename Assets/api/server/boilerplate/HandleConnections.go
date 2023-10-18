package boilerplate

import (
	"fmt"
	"net"

	"github.com/wdlea/aimtrainerAPI/logic"
	. "github.com/wdlea/aimtrainerAPI/objects"
)

const BUFFER_SIZE = 1024
const PACKET_SEPERATOR = '\n'

// Called to handle a singke connection, this blocks so use goroutines to execute this
func HandleConn(conn net.Conn) {
	defer conn.Close() //Make sure to close the connection when finished

	fmt.Printf("New player connection from %s \n", conn.RemoteAddr().String())

	//create the user and destroy it when it goes out of scope(connection lost)
	user := new(Player)
	defer user.Dispose()
	user.Conn = conn

	//set up the channels for trasferring dadt between the threads
	inboundDataChan := make(chan byte, BUFFER_SIZE)
	inboundPacketChan := make(chan Packet, 8)

	outboundPacketChan := make(chan Packet, 8)

	//Execute the channels as goroutines
	go HandleRecieve(inboundDataChan, conn)
	go HandleBytes(inboundDataChan, inboundPacketChan)
	go HandlePackets(inboundPacketChan, outboundPacketChan, user)

	//not this one though becuase the defer calls would execute immediately, if one of the pther threads errors out, it will cascade down to this one and stop it
	HandleSend(outboundPacketChan, user)
}

// Handles retrieving data from incoming sockets
func HandleRecieve(dataChan chan<- byte, conn net.Conn) {
	defer close(dataChan) //make sure to close the channel at the end so i can get the "error cascade" efect down the threads

	for {
		//create the buffer
		buf := make([]byte, BUFFER_SIZE)

		//read data from the socket into the buffer
		n, err := conn.Read(buf)

		//if any errors handle them
		if err == net.ErrClosed {
			//a simple close returns silently
			return
		} else if err != nil {
			//otherwise log it
			fmt.Printf("Error while recieving fron conn: %s, closing... \n", err)
			return
		}

		//resize the buffer based on the number of bytes read
		buf = buf[:n]

		//split it into bytes and send it to the data channel, ready for the next processing step
		for _, b := range buf {
			dataChan <- b
		}
	}
}

// Handles the individual bytes from dataChan and split them up into packets using the delimiter
func HandleBytes(dataChan <-chan byte, packetChan chan<- Packet) {
	//close the channel for the cascade effect when the thread exits
	defer close(packetChan)

	for {
		encodedPacket := make([]byte, 0, 256) //make a buffer for the packet, notice the capacity of 256 bytes, the reccomended

		//read the type byte from the channel
		typeByte, open := <-dataChan

		if typeByte == PACKET_SEPERATOR {
			fmt.Println("Type Byte sent at start of packet")
			continue
		}

		//if the channel has been closed, cascade down
		if !open {
			return
		}

	packetLoop:
		for {
			currentByte, isOpen := <-dataChan

			//if the channel has been closed, cascade down
			if !isOpen {
				return
			}

			if currentByte == PACKET_SEPERATOR {
				//if the byte is the delimeter break the read loop
				break packetLoop
			} else {
				//otherwise append it to the current packet
				encodedPacket = append(encodedPacket, currentByte)
			}
		}

		//once I have read a packet, make a packet struct and send it through the channel to the next stage
		packetChan <- Packet{
			Type:    typeByte,
			Content: encodedPacket,
		}
	}
}

// Handles all the packets created from HandleBytes
func HandlePackets(inbound <-chan Packet, outbound chan<- Packet, user *Player) {
	//close outbound data channel for the cascade effect
	defer close(outbound)

	for {
		// read the next packet
		pak, open := <-inbound

		//if the channel has been closed, stop this thread
		if !open {
			return
		}

		//Handle the packet
		resps, terminate := logic.HandlePacket(pak.Type, pak.Content, user)

		//If the handler says to terminate the connection, do so
		if terminate {
			fmt.Println("Connection terminated by client through intentional behaviour") //connection lost, but cleanly
			return
		}

		//Otherise send the repsonses that the handler wants to send
		for _, resp := range resps {
			fmt.Printf("Responding to packet %s with packet %s\n", pak.ToBytes(0), resp.ToBytes(0))
			outbound <- resp
		}
	}
}

// this handles sending the packets
func HandleSend(outbound <-chan Packet, user *Player) {
	defer fmt.Println("Conn closed")
	for {
		//read from the channel
		currentSend, open := <-outbound

		//if it isnt open cascade down
		if !open {
			return
		}

		user.ConnLock.Lock()
		defer user.ConnLock.Unlock()
		//send the above representation
		n, err := user.Conn.Write(currentSend.ToBytes(PACKET_SEPERATOR))
		fmt.Printf("%d bytes were send to %s", n, user.Name)

		//if there was an error, print it and cascade down
		if err != nil {
			fmt.Printf("Error in writing to connection: %s\n", err.Error())
			return
		}
	}
}
