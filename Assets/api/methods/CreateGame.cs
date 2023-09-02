using System;
using System.Collections;
using System.Collections.Generic;
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
        public static Promise<string> CreateGame()
        {
            Debug.Log("Creating Game");

            Promise<string> promise = new Promise<string>();

            Packet packet = new(PacketType.ServerBoundCreate, "");


            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundCreateResponse,
                onResponse = (Packet p) =>
                  {
                      if (p.type == PacketType.ClientBoundCreateResponse)
                      {
                          GameCode = p.message;
                          promise.Fulfil(p.message);
                      }
                      else
                      {
                          promise.Fail(new UnexpectedPacketException());
                      }
                  }
            };


            IsGameStarted = false;
            IsHost = true;
            Client.EnqueueSend(packet, ticket);

            return promise;
        }
    }
}