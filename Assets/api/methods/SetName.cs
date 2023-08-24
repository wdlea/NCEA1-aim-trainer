using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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