using api.objects;
using System;
using UnityEngine;

namespace api
{
    public static partial class Methods
    {
        /// <summary>
        /// Sends a frame
        /// </summary>
        /// <param name="frame">The frame to send</param>
        /// <returns>A promise that resolves to the game state</returns>
        public static Promise<Game> SendFrame(Frame frame)
        {
            Debug.Log("Sending frame");
            Promise<objects.Game> promise = new Promise<Game>();

            Packet packet = Packet.FromObject(PacketType.ServerBoundFrame, frame);

            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundFrameResponse,
                onResponse = (Packet p) =>
                {
                    if (p.type == PacketType.ClientBoundFrameResponse)
                    {
                        try
                        {
                            promise.Fulfil(JsonUtility.FromJson<Game>(p.message));
                        }
                        catch (Exception e)
                        {
                            promise.Fail(e);
                        }
                    }
                    else
                    {
                        promise.Fail(new UnexpectedPacketException());
                    }
                }
            };

            Client.EnqueueSend(packet, ticket);
            return promise;
        }
    }
}