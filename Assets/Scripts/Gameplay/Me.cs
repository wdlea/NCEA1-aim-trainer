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

    [SerializeField] private float _zOffset = -0.1f;

    private void Start()
    {
        if (Instance != null && Instance != this)
            Debug.LogWarning("More than 1 Me in scene");

        Instance = this;

        CalculateMovement();

        _parentTransform = transform.parent;
        _parentCollider = _parentTransform.GetComponent<Collider>();
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        bool wasHit = _parentCollider.Raycast(ray, out RaycastHit hit, float.PositiveInfinity);

        if (wasHit)
            transform.position = hit.point + new Vector3(0, 0, _zOffset);
        

        Cursor.visible = !wasHit;

        CalculateNormalizedPosition();

        //ClampPosition(); //position will already be clamped becuase the ray wouldn't hit otherwise

        //ApplyNormalizedPosition(); //I don't need to do this unless I allow the server to manipulate the client's position
    }
}