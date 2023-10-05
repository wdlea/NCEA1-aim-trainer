using System.Threading.Tasks;
using UnityEngine;

namespace api {
    public static partial class Methods
    {
        /// <summary>
        /// Joins a game.
        /// </summary>
        /// <param name="code">The code to join the game with</param>
        /// <returns>A promise, will resolve to true if it was sucessful.</returns>
        public static async Task<bool> JoinGame(string code)
        {
            Debug.Log("joining Game");

            Packet packet = new Packet(PacketType.ServerBoundJoin, code);

            Packet response = await Client.SendPacket(
                packet,
                (Packet p) => {
                    return p.Type == PacketType.ClientBoundJoinResponse || p.Type == PacketType.Error;
                }
            );

            return response.Type == PacketType.ClientBoundJoinResponse;
        }
    }
}
