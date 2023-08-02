using System.Collections;
using UnityEngine;

public static partial class Helpers 
{
    public static float ClampWrapping(float value, float minimum, float maximum, float wrapPoint)
    {
        value %= wrapPoint;
        if(maximum - minimum >= wrapPoint - minimum)
        {
            float wrappedMaximum = maximum % wrapPoint;
            if(wrappedMaximum > minimum)
            {
                throw new System.Exception("Wrapped maximum exceeds minimum, L code");
            }else if (value < wrappedMaximum)
            {
                return value;
            }
            else if(value > minimum)
            {
                return value;
            }
            else
            {
                return RoundToNearest(value, minimum, wrappedMaximum);
            }
        }
        else
        {
            return Mathf.Clamp(value, minimum, maximum);
        }
    }
}
