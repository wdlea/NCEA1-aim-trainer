using System.Collections;
using UnityEngine;

/// <summary>
/// The player that I control
/// </summary>
public class Me : Entity
{
    //here to stop double-ups
    protected static Me Instance { get; private set; }

    [SerializeField] protected Camera _camera;
    protected Transform _parentTransform;
    protected Collider _parentCollider;

    private void Start()
    {
        if (Instance != null && Instance != this)
            Debug.LogWarning("More than 1 Me in scene");

        Instance = this;

        StartCoroutine(UpdateDeltaPos());

        _parentTransform = transform.parent;
        _parentCollider = _parentTransform.GetComponent<Collider>();
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        bool wasHit = _parentCollider.Raycast(ray, out RaycastHit hit, float.PositiveInfinity);

        if (wasHit)
        {
            Vector3 hitPosLocal = _parentTransform.InverseTransformPoint(hit.point);

            X = hitPosLocal.x / COORD_RANGE;
            Y = hitPosLocal.y / COORD_RANGE;
        }

        Cursor.visible = !wasHit;

        ClampPosition();

        ApplyPosition();
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