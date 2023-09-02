using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Text;
using UnityEngine;

namespace api
{
    public static partial class Client
    {
        const byte END_OF_PACKET = (byte)'\n';
        const int BUFFER_SIZE = 1024;
        const int MAX_PACKET_LENGTH = 512;

        const int THREAD_SLEEP_TIME = 10;//ms, this will be slept for when the thread is waiting for something to save cpu ticks, increase for less processing power required, but 'coarser' gameplay

        internal static ConcurrentQueue<Packet> recievedPackets;
        internal static ConcurrentQueue<TransmittingPacket> sendQueue;
        internal static ConcurrentQueue<ClaimTicket> recieveClaims;

        public delegate void KillCoroutines();
        internal static KillCoroutines killCoroutines = null;

        /// <summary>
        /// Creates the threads that handle networking.
        /// Subsequent calls restart while clearing all 
        /// means of communication between threads which
        /// data may persist in between restarts.
        /// <param name="surrogate">The monobehaviour to spawn the coroutine on
        /// to allow it to interact with the game thread.</param>
        /// </summary>
        private static void StartThreads(MonoBehaviour surrogate)
        {
            KillThreads();

            sendQueue = new ConcurrentQueue<TransmittingPacket>();
            recievedPackets = new ConcurrentQueue<Packet>();
            recieveClaims = new ConcurrentQueue<ClaimTicket>();

            Coroutine claimsCoroutine = surrogate.StartCoroutine(HandleClaimsCoroutine());
            Coroutine broadcastCoroutine = surrogate.StartCoroutine(HandleBroadcastsCoroutine());

            Thread recievePackets = new Thread(RecievePacketsThread);
            Thread sendPackets = new Thread(SendPacketsThread);


            recievePackets.Priority = System.Threading.ThreadPriority.BelowNormal;
            sendPackets.Priority = System.Threading.ThreadPriority.Normal;

            recievePackets.Start();
            sendPackets.Start();

            killCoroutines = () =>
            {
                killCoroutines = null;

                if (surrogate != null)//if surrogate has been destroyed, so will the coroutines
                {
                    surrogate.StopCoroutine(claimsCoroutine);
                    surrogate.StopCoroutine(broadcastCoroutine);
                }

                recievePackets.Abort();
                sendPackets.Abort();
            };
        }

        /// <summary>
        /// Kills running threads and coroutines
        /// </summary>
        public static void KillThreads()
        {
            killCoroutines?.Invoke();
        }

        /// <summary>
        /// Handles the recieving of packets from the server. 
        /// </summary>
        private static void RecievePacketsThread()
        {
            List<byte> currentPacket = new List<byte>(MAX_PACKET_LENGTH);

            while (IsConnected)
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
                            Packet recieved = new Packet(currentPacket.ToArray());

                            Debug.Log("Recieved Packet" + recieved.ToString());

                            recievedPackets.Enqueue(recieved);
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

                Thread.Sleep(THREAD_SLEEP_TIME);
            }
        }

        /// <summary>
        /// Handles the sending of packets to the server. 
        /// </summary>
        private static void SendPacketsThread()
        {
            while (IsConnected)
            {
                if (sendQueue.TryDequeue(out TransmittingPacket toSend))
                {
                    Send(toSend.packet);
                    recieveClaims.Enqueue(toSend.ticket);
                }

                Thread.Sleep(THREAD_SLEEP_TIME);
            }
        }

        /// <summary>
        /// Handles the claims of packets from the recieve queues. 
        /// </summary>
        private static IEnumerator HandleClaimsCoroutine()
        {
            while (IsConnected)
            {
                if (recieveClaims.TryDequeue(out ClaimTicket ticket))
                {
                    Packet claimedPacket = default;
                    do
                    {
                        if (recievedPackets.Count > 0)
                        {
                            claimedPacket = (
                                from Packet packet in recievedPackets
                                where packet.type == ticket.expectedType || packet.type == PacketType.Error
                                select packet
                            ).FirstOrDefault();
                        }
                        else
                        {
                            yield return null;//do as many packets as possible before surrendering control of thread
                        }

                    } while (claimedPacket == default);

                    ticket.onResponse(claimedPacket);
                }
                else yield return null;
            }
        }

        private static IEnumerator HandleBroadcastsCoroutine()
        {
            while (IsConnected)
            {
                yield return null;

                IEnumerable<Packet> broadcastPackets =
                    (
                        from Packet p in recievedPackets
                        where p.type == PacketType.ClientBoundBroadcast
                        select p
                    );

                recievedPackets =
                    new ConcurrentQueue<Packet>(
                        from Packet p in recievedPackets
                        where p.type != PacketType.ClientBoundBroadcast
                        select p
                    );
                
                foreach(Packet packet in broadcastPackets)
                {
                    //transform packet
                    Packet newPacket = new Packet(Encoding.ASCII.GetBytes(packet.message));
                    Methods.HandleBroadcast(newPacket);
                }
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
    
    /// <summary>
    /// A delegate that is called when a response is recieved for
    /// a sent packet
    /// </summary>
    /// <param name="response">The response packet.</param>
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

        ServerBoundName = 'n',
        ClientBoundNameResponse = 'N',

        ServerBoundJoin = 'j',
        ClientBoundJoinResponse = 'J',

        ServerBoundLeave = 'l',
        ClientBoundLeaveResponse = 'L',

        ServerBoundCreate = 'c',
        ClientBoundCreateResponse = 'C',

        ClientBoundBroadcast = 'B',

        BroadcastStartGame = 'S',

        ServerBoundTerminate = 't',
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
            Encoding.ASCII.GetString(packet[1..]))
        { }

        public static Packet FromObject(PacketType type, object message)
        {
            return new Packet(type, JsonUtility.ToJson(message));
        }

        public byte[] ToBytes()
        {
            byte[] encodedType = Encoding.ASCII.GetBytes(
                    new[] { (char)type }
            );

            byte[] encodedMessage = Encoding.ASCII.GetBytes(
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