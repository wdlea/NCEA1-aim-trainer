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

    protected static RectTransform me;
    protected static RectTransform parent;
    protected static Canvas gameCanvas;

    protected static float XScale => parent.rect.width * gameCanvas.scaleFactor / (MAX_COORD - MIN_COORD);
    protected static float YScale => parent.rect.height * gameCanvas.scaleFactor / (MAX_COORD - MIN_COORD);
    protected static Vector3 GameAreaPosition => parent.position;

    protected void ClampPosition()
    {
        X = Mathf.Clamp(X, MIN_COORD, MAX_COORD);
        Y = Mathf.Clamp(Y, MIN_COORD, MAX_COORD);
    }

    protected void ApplyPosition()
    {
        transform.position = new Vector3(X * XScale, Y * YScale, 0) + GameAreaPosition;
    }
}
