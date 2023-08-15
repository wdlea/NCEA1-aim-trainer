using UnityEngine;

public static partial class Helpers 
{
    public static float ClampWrapping(float value, float minimum, float maximum, float wrapPoint)
    {
        value   = Helpers.Modulo(value, wrapPoint);

        if(maximum < minimum)
        {
            maximum += wrapPoint;
        }
        else if(minimum == maximum)
        {
            return minimum;
        }
        if(wrapPoint == 0)
        {
            throw new System.InvalidOperationException("cannot have a wrap point of 0");
        }

        if((value <= maximum ) && (value >= minimum))
        {
            return value;
        }
        else
        {
            float backDist = Mathf.Sign(minimum - value) == 1 ? minimum - value : minimum + wrapPoint - value;
            float forwardDist = Mathf.Sign(value - maximum) == 1 ? value - maximum : value + wrapPoint - maximum;

            if(backDist < forwardDist)//default to going forward(rounding to highest value)
            {
                return Modulo(minimum, 360);
            }
            else//forward
            {
                return Modulo(maximum, 360);
            }
        }

    }
}
