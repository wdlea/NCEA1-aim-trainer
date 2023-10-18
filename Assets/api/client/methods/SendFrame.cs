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
        public static async Task<Game> SendFrame(Frame frame)
        {
            Debug.Log("Sending frame");
            Packet packet = Packet.FromObject(PacketType.ServerBoundFrame, frame);

            Packet response = await Client.SendPacket(
                packet,
                (Packet p) =>
                {
                    Debug.Log("Checking packet of type " + p.Type + " for frame");
                    return p.Type == PacketType.ClientBoundFrameResponse || p.Type == PacketType.Error;
                }
            );

            if (response.Type == PacketType.ClientBoundFrameResponse)
            {
                FrameCount++;

                return JsonUtility.FromJson<Game>(response.Content);
            }
            else throw new UnexpectedPacketException();
        }

        /// <summary>
        /// Waits for the next frame to be recieved from the server successfully
        /// </summary>
        /// <returns>The count of frames so far.</returns>
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