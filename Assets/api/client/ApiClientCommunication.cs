using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace api
{
    public static partial class Client
    {
        const byte END_OF_PACKET = (byte)'\n'; 
        const int BUFFER_SIZE = 1024;
        const int MAX_PACKET_LENGTH = 512;

        internal static Queue<Packet> recievedPackets = new Queue<Packet>();
        internal static Queue<Packet> sendQueue = new Queue<Packet>();

        public delegate void KillCoroutines();
        internal static KillCoroutines killCoroutines = null;

        /// <summary>
        /// Spawns and respawns coroutines on a given gameobject. 
        /// This also takes care of old coroutines that may or may
        /// not be running. This will reset the current buffers and 
        /// may cause corruption, this should only happen when changing
        /// servers.
        /// </summary>
        /// <param name="surrogate">The gameobject to spawn the coroutines on.</param>
        public static void SpawnCoroutines(MonoBehaviour surrogate)
        {
            killCoroutines?.Invoke();

            Coroutine recv = surrogate.StartCoroutine(RecievePacketsCoroutine());
            Coroutine send = surrogate.StartCoroutine(SendPacketsCoroutine());

            killCoroutines = () =>
            {
                surrogate.StopCoroutine(recv);
                surrogate.StopCoroutine(send);
                killCoroutines = null;
            };
        }

        /// <summary>
        /// Handles the recieving of packets from the server. This will be executed in the game via a "surrogate" gameobject.
        /// </summary>
        /// <returns>The coroutine</returns>
        internal static IEnumerator RecievePacketsCoroutine()
        {
            List<byte> currentPacket = new List<byte>(MAX_PACKET_LENGTH);

            while (Connected)
            {
                if(communicationSocket.Available > 0)
                {
                    byte[] buf = new byte[BUFFER_SIZE];
                    int n = communicationSocket.Receive(buf);
                    buf = buf[..n];
                    
                    foreach(byte b in buf)
                    {
                        if(b == END_OF_PACKET)
                        {
                            recievedPackets.Enqueue(
                                new Packet(currentPacket.ToArray())
                            );
                            currentPacket.Clear();
                        }
                        else
                        {
                            currentPacket.Add(b);
                            if(currentPacket.Count > MAX_PACKET_LENGTH)
                            {
                                throw new OversizedPacketException();
                            }
                        }
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Handles the sending of packets to the server. This will be executed in the game via a "surrogate" gameobject.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerator SendPacketsCoroutine()
        {
            while (Connected)
            {
                if(recievedPackets.TryDequeue(out Packet toSend))
                {
                    Send(toSend);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Sends a packet to the server, do not call this,
        /// instead add the packet to the sendQueue.
        /// </summary>
        /// <param name="packet"></param>
        internal static void Send(Packet packet)
        {
            communicationSocket.Send(packet.ToBytes());
            communicationSocket.Send(new[] { END_OF_PACKET });
        }
    }

    internal enum PacketType
    {
        Error = 'E',

        ServerBoundFrame = 'f',
        ClientBoundFrameResponse = 'F',


    }

    [Serializable]
    internal struct Packet
    {
        public readonly PacketType type;

        public readonly string message;


        public Packet(PacketType type, string message)
        {
            this.type = type;
            this.message = message;
        }

        public Packet(byte[] packet) : this(
            (PacketType)(char)packet[0], 
            Encoding.Unicode.GetString(packet[1..])) { }

        public static Packet FromObject(PacketType type, object message)
        {
            return new Packet(type, JsonUtility.ToJson(message));
        }

        public byte[] ToBytes()
        {
            byte[] encodedType = Encoding.Unicode.GetBytes(
                    new [] {(char)type}
            );

            byte[] encodedMessage = Encoding.Unicode.GetBytes(
                    message
            );


            return encodedType.Concat(encodedMessage).ToArray();
        }
    }   
}