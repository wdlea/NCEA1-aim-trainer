namespace api
{
    public static partial class Methods
    {
        /// <summary>
        /// Handles a broadcast packet
        /// </summary>
        /// <param name="newPacket">The broadcast packet, 
        /// note that this will be transfomed to omit the
        /// first 'B'</param>
        internal static void HandleBroadcast(Packet newPacket)
        {
            //do stuff with packet
        }
    }
}
