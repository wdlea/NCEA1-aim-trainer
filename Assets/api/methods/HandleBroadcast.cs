using UnityEngine;

namespace api
{
    public static partial class Methods
    {
        public static bool IsGameStarted { get; private set; }

        public enum Broadcast
        {
            StartGame = 'S',
            SpawnTarget = 'T',
        }

        /// <summary>
        /// Handles a broadcast packet
        /// </summary>
        /// <param name="newPacket">The broadcast packet, 
        /// note that this will be transfomed to omit the
        /// first 'B'</param>
        internal static void HandleBroadcast(Packet newPacket)
        {
            //do stuff with packet
            switch ((Broadcast)newPacket.type)
            {
                case Broadcast.StartGame:
                    {
                        Debug.Log("Started Game");
                        IsGameStarted = true;
                        break;
                    }
                case Broadcast.SpawnTarget:
                    {

                        break;
                    }


                default:
                    {
                        throw new UnexpectedPacketException();
                    }
            }
        }
    }
}
