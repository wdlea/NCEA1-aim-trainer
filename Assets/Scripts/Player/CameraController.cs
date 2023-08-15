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
    Vector3 rotationEuler;
    private void UpdateRotation()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 delta = mousePos - prevPos;

        prevPos = mousePos;

        delta *= sens;

        rotationEuler +=
            (Quaternion.AngleAxis(delta.x, Vector3.up) *
            Quaternion.AngleAxis(delta.y, Vector3.right))
            .eulerAngles;




        //rotationEuler.x = Helpers.ClampWrapping(rotationEuler.x, MinYRotation, MaxYRotation, 360f);
        //rotationEuler.y = Helpers.ClampWrapping(rotationEuler.y, MinXRotation, MaxXRotation, 360f);



        transform.rotation = Quaternion.Euler(rotationEuler);
    }

    private void Shoot()
    {
        Debug.Log("pew");
        if(Physics.Raycast(lookRay, out RaycastHit hit))
        {
            foreach(Shootables.Shootable shoot in hit.transform.GetComponents<Shootables.Shootable>())
            {
                shoot.OnHit();
            }
        }
    }
}
