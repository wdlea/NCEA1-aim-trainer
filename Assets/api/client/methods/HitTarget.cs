using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace api
{
    public static partial class Methods
    {
        /// <summary>
        /// Marks a target as "hit" on the server
        /// </summary>
        /// <param name="ID">The id of the target I shot</param>
        public static async Task HitTarget(int ID)
        {
            Packet packet = new Packet(
                PacketType.ServerBoundHitTarget,
                ID.ToString()
            );


            //I don't really care what happens
            await Client.SendPacket(
                packet,
                (Packet p) => {
                    return p.Type == PacketType.ClientBoundHitTargetResponse || p.Type == PacketType.Error;
                }
            );
        }
    }
}
