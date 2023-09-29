using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity is any object that's position is partially or fully synchronised with the server using frames
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class Entity : MonoBehaviour
{
    public const float MIN_COORD = -100f;
    public const float MAX_COORD = 100f;
    public const float COORD_RANGE = MAX_COORD - MIN_COORD;

    public float X, Y, Dx, Dy;

    public api.objects.Frame Frame
    {
        get => new api.objects.Frame { X = X, Y = Y, Dx = Dx, Dy = Dy };
        set
        {
            X = value.X;
            Y = value.Y;
            Dx = value.Dx;
            Dy = value.Dy;
        }
    }

    protected void ClampPosition()
    {
        X = Mathf.Clamp(X, MIN_COORD, MAX_COORD);
        Y = Mathf.Clamp(Y, MIN_COORD, MAX_COORD);
    }

    protected void ApplyPosition()
    {
        Transform parent = transform.parent;
        transform.localPosition = new Vector3(X * COORD_RANGE, Y * COORD_RANGE, -0.1f);
    }
}
