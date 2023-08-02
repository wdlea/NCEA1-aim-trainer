using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownTarget : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 forceLowerBound;
    [SerializeField] private Vector3 forceUpperBound;
    [SerializeField] private Vector3 startPositionLowerBound;
    [SerializeField] private Vector3 startPositionUpperBound;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Helpers.RandomVector3Range(startPositionLowerBound, startPositionUpperBound);
        
        Vector3 velocity = Helpers.RandomVector3Range(forceLowerBound, forceUpperBound);
        rb.velocity = velocity;
    }
}
