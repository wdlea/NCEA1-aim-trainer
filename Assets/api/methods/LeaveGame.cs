namespace api { 
    public static partial class Methods
    {

        /// <summary>
        /// Leaves the game the player is currently in.
        /// </summary>
        /// <returns>A promise resolving to true when the game has been left</returns>
        public static Promise<bool> LeaveGame()
        {
            Promise<bool> promise = new();

            Packet packet = new(PacketType.ServerBoundLeave, "");

            ClaimTicket ticket = new ClaimTicket
            {
                expectedType = PacketType.ClientBoundLeaveResponse,
                onResponse = (Packet p) =>
                  {
                      if (p.type == PacketType.ClientBoundLeaveResponse)
                      {
                          promise.Fulfil(true);
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