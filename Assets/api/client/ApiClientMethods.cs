using System.Net;
using UnityEngine;

namespace api
{
    public static partial class Client
    {
        const string SERVER_ADDR = "127.0.0.1";
        const int SERVER_PORT = 80;

        public static void EnqueueSend(Packet packet, ClaimTicket ticket)
        {
            sendQueue.Enqueue(
                new TransmittingPacket
                {
                    packet = packet,
                    ticket = ticket
                }
            );
        }

        public static void JoinServer(MonoBehaviour surrogate)
        {
            
            ServerEndpoint = new IPEndPoint(IPAddress.Parse(SERVER_ADDR), SERVER_PORT);
            SpawnCoroutines(surrogate);
        }
    }
}