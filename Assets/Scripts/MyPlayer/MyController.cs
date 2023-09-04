using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MyController : MonoBehaviour
{
    const float MIN_COORD = -100f;
    const float MAX_COORD = 100f;

    public float X, Y, Dx, Dy;

    RectTransform me;
    RectTransform parent;
    Canvas gameCanvas;

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
        me = GetComponent<RectTransform>();
        parent = me.parent.GetComponent<RectTransform>();
        gameCanvas = me.GetComponentInParent<Canvas>();

        StartCoroutine(UpdateDeltaPos());
    }

    private void Update()
    {
        X = Input.mousePosition.x;
        Y = Input.mousePosition.y;

        float xScale = parent.rect.width * gameCanvas.scaleFactor / (MAX_COORD - MIN_COORD);
        float yScale = parent.rect.height * gameCanvas.scaleFactor / (MAX_COORD - MIN_COORD);

        X -= me.parent.position.x;
        Y -= me.parent.position.y;

        X /= xScale;
        Y /= yScale;

        X = Mathf.Clamp(X, MIN_COORD, MAX_COORD);
        Y = Mathf.Clamp(Y, MIN_COORD, MAX_COORD);

        transform.position = new Vector3(X * xScale, Y * yScale, 0) + me.parent.position;
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