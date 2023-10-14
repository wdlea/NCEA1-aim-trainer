using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#nullable enable
public class TargetHitter : MonoBehaviour
{
    private Camera? _main;
    [SerializeField] private LayerMask targetLayers;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            _main ??= Camera.main;

            Ray ray = _main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, targetLayers)){
                Target t = hit.transform.GetComponent<Target>();
                t?.OnHit();
            }
        }
    }
}
