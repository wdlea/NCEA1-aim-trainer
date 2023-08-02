using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]float MinXRotation = -45;
    [SerializeField]float MaxXRotation = 45;
    [SerializeField]float MinYRotation = -45;
    [SerializeField]float MaxYRotation = 85;

    [SerializeField]float sens = 1.0f;

    [SerializeField]int shootButton = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Ray lookRay => new Ray(transform.position, transform.rotation * Vector3.forward);

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
        if (Input.GetMouseButtonDown(shootButton))
            Shoot();
    }

    Vector2 prevPos;
    private void UpdateRotation()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 delta = mousePos - prevPos;

        Debug.Log(delta);
        prevPos = mousePos;

        delta *= sens;

        Vector3 newRot = transform.rotation.eulerAngles + new Vector3(delta.y, delta.x, 0f);
        newRot.x = Mathf.Clamp(newRot.x, MinYRotation, MaxYRotation);
        newRot.y = Mathf.Clamp(newRot.y, MinXRotation, MaxXRotation);

        

        transform.rotation = Quaternion.Euler(newRot);
    }

    private void Shoot()
    {
        if(Physics.Raycast(lookRay, out RaycastHit hit))
        {
            foreach(Shootables.Shootable shoot in hit.transform.GetComponents<Shootables.Shootable>())
            {
                shoot.OnHit();
            }
        }
    }
}
