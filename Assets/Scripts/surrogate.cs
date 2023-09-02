using api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A surrogate is a gameobject that is not destroyed.
/// It is intended for you to spawn your coroutines on.
/// </summary>
public class surrogate : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        Client.KillThreads();
    }
}
