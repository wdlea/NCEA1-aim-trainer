using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace api
{
    public static partial class Methods
    {
        /// <summary>
        /// Marks a target as "hit" on the server
        /// </summary>
        /// <param name="ID">The id of the target I shot</param>
        public static void HitTarget(int ID)
        {
            Packet packet = new Packet(
                PacketType.ServerBoundHitTarget,
                ID.ToString()
            );
            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundHitTargetResponse,
                onResponse = (Packet p) => { }
            };

            Client.EnqueueSend(packet, ticket);
        }
    }
}
