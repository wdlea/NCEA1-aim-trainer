using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
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

    private void Update()
    {
        X += Dx * Time.deltaTime;
        Y += Dy * Time.deltaTime;

        X = Mathf.Clamp(X, MyController.MIN_COORD, MyController.MAX_COORD);
        Y = Mathf.Clamp(Y, MyController.MIN_COORD, MyController.MAX_COORD);

        transform.position = new Vector3(X * MyController.XScale, Y * MyController.YScale, 0) + MyController.GameAreaPosition;
    }
}
