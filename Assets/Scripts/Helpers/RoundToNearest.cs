using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Helpers
{
    /// <summary>
    /// Rounds to the nearest value out of a or b, defaults to a in the case of equal distances
    /// </summary>
    /// <param name="value">The value to round</param>
    /// <param name="a">One point to round to</param>
    /// <param name="b">The other points</param>
    /// <returns></returns>
    public static float RoundToNearest(float value, float a, float b)
    {
        bool aClosest = Mathf.Abs(value - a) <= Mathf.Abs(value - b);

        return aClosest ? a : b;
    }
}
