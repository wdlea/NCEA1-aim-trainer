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

    /// <summary>
    /// Represents the movement of a player in an instant
    /// </summary>
    [Serializable]
    public class Frame
    {
        public float X, Y, Dx, Dy;
    }
}
