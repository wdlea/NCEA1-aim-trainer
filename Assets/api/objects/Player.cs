using System;

namespace api.objects
{
    /// <summary>
    /// Represents a player in the game, used to unmarshal JSON from game
    /// </summary>
    [Serializable]
    public class Player : Frame
    {
        public string Name;
    }
}
