using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    const float PROJECTED_COORD_RANGE = 1f;
    const float PROJECTION_SCALE_FACTOR = PROJECTED_COORD_RANGE / COORD_RANGE;

    const float VELOCITY_DAMPING_FACTOR = 0.5f;
    const float ACCELLERATION_DAMPING_FACTOR = 0.5f;

    public float X, Y, Dx, Dy, DDx, DDy;

    public api.objects.Frame Frame
    {
        get => new api.objects.Frame { X = X, Y = Y, Dx = Dx, Dy = Dy };
        set
        {
            X = value.X;
            Y = value.Y;
            Dx = value.Dx;
            Dy = value.Dy;
            DDx = value.DDx;
            DDy = value.DDy;
        }
    }

    protected void ClampPosition()
    {
        X = Mathf.Clamp(X, MIN_COORD, MAX_COORD);
        Y = Mathf.Clamp(Y, MIN_COORD, MAX_COORD);
    }

    protected void ApplyNormalizedPosition()
    {
        transform.localPosition = new Vector3(X * PROJECTION_SCALE_FACTOR, Y * PROJECTION_SCALE_FACTOR, -0.1f);
    }

    protected void CalculateNormalizedPosition()
    {
        X = transform.localPosition.x / PROJECTION_SCALE_FACTOR;
        Y = transform.localPosition.y / PROJECTION_SCALE_FACTOR;
    }

    protected async void CalculateMovement()
    {
        while (this != null)//while this gameobject hasn't been destroyed
        {
            float pX=X, pY=Y, pDx = Dx, pDy = Dy;

            await Task.Yield();

            Dx = (X - pX) / Time.deltaTime;
            Dy = (Y - pY) / Time.deltaTime;

            DDx = (Dx - pDx) / Time.deltaTime;
            DDy = (Dy - pDy) / Time.deltaTime;
        }
    }

    protected void ApplyMovement()
    {
        Dx *= VELOCITY_DAMPING_FACTOR / Time.deltaTime;
        Dy *= VELOCITY_DAMPING_FACTOR / Time.deltaTime;

        DDx *= ACCELLERATION_DAMPING_FACTOR / Time.deltaTime;
        DDy *= ACCELLERATION_DAMPING_FACTOR / Time.deltaTime;

        Dx += DDx * Time.deltaTime;
        Dy += DDy * Time.deltaTime;

        X += Dx * Time.deltaTime;
        Y += Dy * Time.deltaTime;
    }
}
