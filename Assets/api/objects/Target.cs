using System.Collections;
using UnityEngine;
using System;

namespace api.objects
{
    /// <summary>
    /// Used to deserialize the Target obejct from the server
    /// </summary>
    [Serializable]
    public class Target
    {
        public float X, Y, Dx, Dy;
        public int ID;
    }
}