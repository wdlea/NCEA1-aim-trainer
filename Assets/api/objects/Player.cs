using System;

namespace api.objects
{
    [Serializable]
    public class Player : Frame
    {
        public string Name;
    }

    [Serializable]
    public class Frame
    {
        public float X, Y, Dx, Dy;
    }
}
