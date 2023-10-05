using System.Threading.Tasks;
using UnityEngine;

namespace api { 
    public static partial class Methods
    {

        /// <summary>
        /// Leaves the game the player is currently in.
        /// </summary>
        /// <returns>A promise resolving to true when the game has been left</returns>
        public static async Task LeaveGame()
        {
            Packet packet = new(PacketType.ServerBoundLeave, "");

            Packet response = await Client.SendPacket(
                packet,
                (Packet p) =>
                {
                    return p.Type == PacketType.ClientBoundLeaveResponse || p.Type == PacketType.Error;
                }
            );

            if (response.Type != PacketType.ClientBoundLeaveResponse)
                return;
            else throw new UnexpectedPacketException();
        }
    }
}