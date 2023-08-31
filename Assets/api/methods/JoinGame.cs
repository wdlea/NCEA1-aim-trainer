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
            Debug.Log("joining Game");
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

            GameStarted = false;
            Client.EnqueueSend(packet, ticket);

            return promise;
        }
    }
}
