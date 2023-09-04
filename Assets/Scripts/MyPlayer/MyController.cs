using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MyController : MonoBehaviour
{
    public const float MIN_COORD = -100f;
    public const float MAX_COORD = 100f;

    public float X, Y, Dx, Dy;

    static RectTransform me;
    static RectTransform parent;
    static Canvas gameCanvas;

    public static float XScale => parent.rect.width * gameCanvas.scaleFactor / (MAX_COORD - MIN_COORD);
    public static float YScale => parent.rect.height * gameCanvas.scaleFactor / (MAX_COORD - MIN_COORD);
    public static Vector3 GameAreaPosition => parent.position;

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
        if (me != null && me.gameObject != gameObject)
            Debug.LogWarning("More than 1 mycontroller in scene");

        me = GetComponent<RectTransform>();
        parent = me.parent.GetComponent<RectTransform>();
        gameCanvas = me.GetComponentInParent<Canvas>();

        StartCoroutine(UpdateDeltaPos());
    }

    private void Update()
    {
        X = Input.mousePosition.x;
        Y = Input.mousePosition.y;

        X -= GameAreaPosition.x;
        Y -= GameAreaPosition.y;

        X /= XScale;
        Y /= YScale;

        //hide cursor if in game
        Cursor.visible = X < MIN_COORD || X > MAX_COORD || Y < MIN_COORD || Y > MAX_COORD;

        X = Mathf.Clamp(X, MIN_COORD, MAX_COORD);
        Y = Mathf.Clamp(Y, MIN_COORD, MAX_COORD);

        transform.position = new Vector3(X * XScale, Y * YScale, 0) + GameAreaPosition;
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