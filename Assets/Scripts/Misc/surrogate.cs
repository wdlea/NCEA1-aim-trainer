using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A surrogate is a gameobject that is not destroyed.
/// It is intended for you to spawn your coroutines on.
/// </summary>
public class Surrogate : MonoBehaviour
{
    public static Surrogate Instance { get; private set; }

    public delegate void Call();
    public static Queue<Call> onQuit;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Debug.LogWarning("More than 1 instance of surrogate in scene");

        Instance = this;

        onQuit ??= new Queue<Call>();

        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        foreach (Call call in onQuit)
        {
            call();
        }
    }
}
