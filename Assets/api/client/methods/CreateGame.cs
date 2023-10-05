using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace api
{
    public static partial class Methods
    {

        public static bool IsHost { get; private set; }

        public static string? GameCode { get; private set; }//todo make this the only isntance of code

        /// <summary>
        /// Creates a game.
        /// </summary>
        /// <returns>A promise resolving to the game code</returns>
        public static async Task<string> CreateGame()
        {
            Packet packet = new(PacketType.ServerBoundCreate, "");

            Packet response = await Client.SendPacket(
                packet,
                (Packet p) =>
                {
                    return p.Type == PacketType.ClientBoundCreateResponse || p.Type == PacketType.Error;
                }
            );

            if (response.Type != PacketType.ClientBoundCreateResponse)
                throw new UnexpectedPacketException();

            return response.Content;
        }
    }
}