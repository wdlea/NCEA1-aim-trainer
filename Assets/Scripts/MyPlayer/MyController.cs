using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyController : MonoBehaviour
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

    private void Start()
    {
        StartCoroutine(UpdateDeltaPos());
    }

    private void Update()
    {
        X = Input.mousePosition.x;
        Y = Input.mousePosition.y;

        transform.position = new Vector3(X, Y, 0);
    }

    private IEnumerator UpdateDeltaPos()
    {
        while(true){
            float pX = X;
            float pY = Y;

            yield return new WaitForSeconds(1 / 5);//5hz

            Dx = (X - pX) / Time.deltaTime;
            Dy = (Y - pY) / Time.deltaTime;
        }
    }
}