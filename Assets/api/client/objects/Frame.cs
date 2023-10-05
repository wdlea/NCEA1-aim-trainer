using System;

namespace api.objects
{
    /// <summary>
    /// Represents the movement of a player in an instant
    /// </summary>
    [Serializable]
    public class Frame
    {
        public float X, Y, Dx, Dy, DDx, DDy;
    }
}
