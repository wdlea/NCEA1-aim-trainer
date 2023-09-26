using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows multliple Graphics to be toggled at once, can also be used as an 
/// adapter for non-graphic objects;
/// </summary>
public class GraphicGroup: Graphic {
     [SerializeField] private MonoBehaviour[] grouped;

    protected override void OnEnable()
    {
        foreach(MonoBehaviour behaviour in grouped)
        {
            behaviour.enabled = true;
        }
    }

    protected override void OnDisable()
    {
        foreach (MonoBehaviour behaviour in grouped)
        {
            behaviour.enabled = false;
        }
    }
}
