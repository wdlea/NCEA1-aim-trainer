#nullable enable

using System.Collections.Generic;

namespace api
{
    public static partial class Client
    {
        private static List<IPlugin> _plugins = new();

        private static void AddPlugins()
        {
            _plugins.Clear();
            _plugins.Add(new Plugins.Broadcasts());
        }

        private static Packet? ApplyPlugins(Packet p)
        {
            Packet? currentPacket = p;

            foreach(IPlugin plugin in _plugins)
            {
                if (currentPacket is null)
                    break;
                currentPacket = plugin.Process(currentPacket);
            }

            return currentPacket;
        }

        /// <summary>
        /// Represents a plugin, a piece of code that processes packets before they are distributed
        /// </summary>
        public interface IPlugin
        {
            /// <summary>
            /// Processes a packet
            /// </summary>
            /// <param name="p">The packet to process</param>
            /// <returns>The modified packet, or null to signify that the packet was destroyed</returns>
            public Packet? Process(Packet p);
        }
    }
}
