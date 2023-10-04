using System.Threading.Tasks;
using UnityEngine;

namespace api
{
    public static partial class Methods
    {
        /// <summary>
        /// Sets the player name, DO NOT apply 
        /// the changed name before the promise resolves
        /// the server is written to obey FIFO order.
        /// </summary>
        /// <param name="name">The name to set</param>
        /// <returns>A promise resolving to the set name</returns>
        public static async Task<string> SetName(string name)
        {
            Packet packet = new Packet(PacketType.ServerBoundName, name);

            Packet response = await Client.SendPacket(
                packet,
                (Packet p) =>
                {
                    return p.Type == PacketType.ClientBoundNameResponse || p.Type == PacketType.Error;
                }
            );

            return response.Content;
        }
    }
}