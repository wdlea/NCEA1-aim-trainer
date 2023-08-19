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
        internal static Queue<TransmittingPacket> sendQueue = new Queue<TransmittingPacket>();
        internal static Queue<ClaimTicket> recieveClaims = new Queue<ClaimTicket>();

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
        internal static void SpawnCoroutines(MonoBehaviour surrogate)
        {
            if (surrogate == null) return;//if the surrogate has been destroyed, i can't spawn any coroutines

            killCoroutines?.Invoke();

            Coroutine recv = surrogate.StartCoroutine(RecievePacketsCoroutine());
            Coroutine send = surrogate.StartCoroutine(SendPacketsCoroutine());
            Coroutine claim = surrogate.StartCoroutine(HandleClaimsCoroutine());

            killCoroutines = () =>
            {
                killCoroutines = null;//this can only be run once

                if (surrogate == null) return;//if the surrogate has been destroyed, so will the coroutines

                surrogate.StopCoroutine(recv);
                surrogate.StopCoroutine(send);
                surrogate.StopCoroutine(claim);
            };
        }

        /// <summary>
        /// Handles the recieving of packets from the server. This will be executed in the game via a "surrogate" gameobject.
        /// </summary>
        /// <returns>The coroutine</returns>
        private static IEnumerator RecievePacketsCoroutine()
        {
            List<byte> currentPacket = new List<byte>(MAX_PACKET_LENGTH);

            while (Connected)
            {
                if (communicationSocket.Available > 0)
                {
                    byte[] buf = new byte[BUFFER_SIZE];
                    int n = communicationSocket.Receive(buf);
                    buf = buf[..n];

                    foreach (byte b in buf)
                    {
                        if (b == END_OF_PACKET)
                        {
                            recievedPackets.Enqueue(
                                new Packet(currentPacket.ToArray())
                            );
                            currentPacket.Clear();
                        }
                        else
                        {
                            currentPacket.Add(b);
                            if (currentPacket.Count > MAX_PACKET_LENGTH)
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
        private static IEnumerator SendPacketsCoroutine()
        {
            while (Connected)
            {
                if (sendQueue.TryDequeue(out TransmittingPacket toSend))
                {
                    Send(toSend.packet);
                    recieveClaims.Enqueue(toSend.ticket);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Handles the claims of packets from the recieve queues. This will be executed in the game via a "surrogate" gameobject.
        /// </summary>
        /// <returns></returns>
        private static IEnumerator HandleClaimsCoroutine()
        {
            while (Connected)
            {
                if (recieveClaims.TryDequeue(out ClaimTicket ticket))
                {
                    Packet claimedPacket = default;
                    do
                    {
                        claimedPacket = (
                            from Packet packet in recievedPackets
                            where packet.type == ticket.expectedType || packet.type == PacketType.Error
                            select packet
                        ).First();

                        yield return null;
                    } while (claimedPacket == default);

                    ticket.onResponse(claimedPacket);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Sends a packet to the server, do not call this,
        /// instead add the packet to the sendQueue.
        /// </summary>
        /// <param name="packet"></param>
        private static void Send(Packet packet)
        {
            communicationSocket.Send(packet.ToBytes());
            communicationSocket.Send(new[] { END_OF_PACKET });
        }
    }

    public delegate void OnResponse(Packet response);

    /// <summary>
    /// A packet in the send queue, contains
    /// a packet and ticket.
    /// </summary>
    internal struct TransmittingPacket
    {
        public Packet packet;
        public ClaimTicket ticket;
    }

    /// <summary>
    /// A ticket that instructs the client what 
    /// to do with the response it gets.
    /// </summary>
    public struct ClaimTicket
    {
        public OnResponse onResponse;
        public PacketType expectedType;
    }

    /// <summary>
    /// The type of packet.
    /// </summary>
    public enum PacketType
    {
        Error = 'E',

        ServerBoundFrame = 'f',
        ClientBoundFrameResponse = 'F',


    }

    /// <summary>
    /// A packet is a single "unit"
    /// that gets sent to the server.
    /// </summary>
    [Serializable]
    public struct Packet
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
            Encoding.Unicode.GetString(packet[1..]))
        { }

        public static Packet FromObject(PacketType type, object message)
        {
            return new Packet(type, JsonUtility.ToJson(message));
        }

        public byte[] ToBytes()
        {
            byte[] encodedType = Encoding.Unicode.GetBytes(
                    new[] { (char)type }
            );

            byte[] encodedMessage = Encoding.Unicode.GetBytes(
                    message
            );


            return encodedType.Concat(encodedMessage).ToArray();
        }


        public static bool operator ==(Packet left, Packet right)
        {
            return (left.message == right.message) && (left.type == right.type);
        }
        public static bool operator !=(Packet left, Packet right)
        {
            return !(left == right);
        }
    }
}