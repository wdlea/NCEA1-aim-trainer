using UnityEngine;

public class LimboSphere : MonoBehaviour
{
    [SerializeField] Vector3 spawnLocation;

    [SerializeField] float interval;
    [SerializeField] float offset;

    Rigidbody rb;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        InvokeRepeating(nameof(TeleportBack), offset, interval);
    }

    void TeleportBack()
    {
        transform.position = spawnLocation;
        rb.velocity = Vector3.zero;
    }

    private void OnMouseDown()
    {
        transform.position = Vector3.one * 9999;
        //maybe play sound
    }
}
