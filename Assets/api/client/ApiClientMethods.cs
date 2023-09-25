using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace api
{
    public static partial class Client
    {
        const string SERVER_ADDR = "10.2.24.143";
        const int SERVER_PORT = 80;

        /// <summary>
        /// Enqueues a packet to be sent.
        /// </summary>
        /// <param name="packet">The packet that will be sent.</param>
        /// <param name="ticket">The ticket to handle the response packet.</param>
        internal static void EnqueueSend(Packet packet, ClaimTicket ticket)
        {
            sendQueue.Enqueue(
                new TransmittingPacket
                {
                    packet = packet,
                    ticket = ticket
                }
            );
        }

        /// <summary>
        /// Joins a server.
        /// </summary>
        /// <param name="surrogate">
        /// A surrogate MonoBehaviour(can literally be 
        /// anything as long as it doesnt get 
        /// destroyed when the server is running) 
        /// to use for spawning coroutines.</param>
        public static void JoinServer()
        {
            ServerEndpoint = new IPEndPoint(IPAddress.Parse(SERVER_ADDR), SERVER_PORT);
            StartThreads(Surrogate.Instance);

            Surrogate.onQuit.Enqueue(KillThreads);
        }
    }
}