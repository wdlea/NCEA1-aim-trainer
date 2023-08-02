using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Helpers
{
    public static void MinMax(in float a, in float b, out float min, out float max)
    {
        if(a > b)
        {
            min = b; 
            max = a;
        }
        else
        {
            max = b;
            min = a;
        }
        return;
    }
}
