using System.Collections;
using UnityEngine;
using System;

namespace api.objects
{
    /// <summary>
    /// Used to deserialize the Target obejct from the server
    /// </summary>
    [Serializable]
    public class Target : Frame
    {
        public int ID;
    }
}