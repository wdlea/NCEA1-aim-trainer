using System.Collections;
using UnityEngine;

/// <summary>
/// The player that I control
/// </summary>
public class Me : Entity
{
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

        ClampPosition();

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