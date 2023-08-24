using api.objects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace api {
    public static partial class Methods
    {
        /// <summary>
        /// Joins a game.
        /// </summary>
        /// <param name="code">The code to join the game with</param>
        /// <returns>A promise, will resolve to true if it was sucessful.</returns>
        public static Promise<bool> JoinGame(string code)
        {
            Promise<bool> promise = new Promise<bool>();

            Packet packet = new Packet(PacketType.ServerBoundJoin, code);

            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundJoinResponse,
                onResponse = (Packet p) =>
                  {
                      if(p.type == PacketType.ClientBoundJoinResponse)
                      {
                          promise.Fulfil(true);
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

        /// <summary>
        /// Sends a frame
        /// </summary>
        /// <param name="frame">The frame to send</param>
        /// <returns>A promise that resolves to the game state</returns>
        public static Promise<Game> SendFrame(Frame frame)
        {
            Promise<objects.Game> promise = new Promise<Game>();

            Packet packet = Packet.FromObject(PacketType.ServerBoundFrame, frame);

            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundFrameResponse,
                onResponse = (Packet p) =>
                  {
                      if(p.type == PacketType.ClientBoundFrameResponse)
                      {
                          try
                          {
                              promise.Fulfil(JsonUtility.FromJson<Game>(p.message));
                          }catch(Exception e){
                              promise.Fail(e);
                          }
                      }
                      else
                      {
                          promise.Fail(new UnexpectedPacketException());
                      }
                  }
            };
            return promise;
        }

        /// <summary>
        /// Sets the player name, DO NOT apply 
        /// the changed name before the promise resolves
        /// the server is written to obey FIFO order.
        /// </summary>
        /// <param name="name">The name to set</param>
        /// <returns>A promise resolving to the set name</returns>
        public static Promise<string> SetName(string name)
        {
            Promise<string> promise = new Promise<string>();

            Packet packet = new Packet(PacketType.ServerBoundName, name);

            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundNameResponse,
                onResponse = (Packet p) =>
                  {
                      if (p.type == PacketType.ClientBoundNameResponse)
                      {
                          promise.Fulfil(p.message);
                      }
                      else
                      {
                          promise.Fail(new UnexpectedPacketException());
                      }
                  }
            };

            return promise;
        }
    }
}