using api.objects;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace api
{
    public static partial class Methods
    {

        public static int FrameCount { get; private set; }
        /// <summary>
        /// Sends a frame
        /// </summary>
        /// <param name="frame">The frame to send</param>
        /// <returns>A promise that resolves to the game state</returns>
        public static Promise<Game> SendFrame(Frame frame)
        {
            Promise<Game> promise = new Promise<Game>();

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
                            promise.Fulfil(JsonUtility.FromJson<Game>(p.content));
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

                    FrameCount++;
                }
            };

            Client.EnqueueSend(packet, ticket);
            return promise;
        }

        public static async Task<int> WaitNextFrame()
        {
            int currentFrameCount = FrameCount;

            while(currentFrameCount == FrameCount)
            {
                await Task.Yield();
            }

            return currentFrameCount;
        }
    }
}