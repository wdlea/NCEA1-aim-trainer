using UnityEngine;

public class LimboSphere : MonoBehaviour
{
    [SerializeField] Vector3 spawnLocation;

    [SerializeField] float interval;
    [SerializeField] float offset;

    Rigidbody rb;

    void Start()
    {
        InvokeRepeating(nameof(TeleportBack), offset, interval);
        rb = GetComponent<Rigidbody>();
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
