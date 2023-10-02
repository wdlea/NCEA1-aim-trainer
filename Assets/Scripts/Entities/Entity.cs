using api;
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
    const float ACCELLERATION_DAMPING_FACTOR = 0.0001f;

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
        float pX = X, pY = Y, pDx = Dx, pDy = Dy, pTime = Time.realtimeSinceStartup;


        while (true)//while this gameobject hasn't been destroyed
        {
            await Methods.WaitNextFrame();

            float dt = Time.realtimeSinceStartup - pTime;

            Dx = (X - pX) / dt;
            Dy = (Y - pY) / dt;

            DDx = (Dx - pDx) / dt;
            DDy = (Dy - pDy) / dt;

            pX = X; pY = Y; pDx = Dx; pDy = Dy; pTime = Time.realtimeSinceStartup;
        }
    }

    protected void ApplyMovement()
    {
        float DFactor = Mathf.Pow(VELOCITY_DAMPING_FACTOR, Time.deltaTime);

        Dx *= DFactor;
        Dy *= DFactor;

        float DDFactor = Mathf.Pow(ACCELLERATION_DAMPING_FACTOR, Time.deltaTime);

        DDx *= DDFactor;
        DDy *= DDFactor;

        if (DDy < 1)
            DDy = 0;

        if (DDx < 1)
            DDx = 0;

        Dx += DDx * Time.deltaTime;
        Dy += DDy * Time.deltaTime;

        X += Dx * Time.deltaTime;
        Y += Dy * Time.deltaTime;
    }
}
