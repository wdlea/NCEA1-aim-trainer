using api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] surrogate surrogate;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(surrogate);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Client.IsConnected)
        {
            Client.JoinServer(surrogate);//blocks until joined so it will work or throw error
        }
    }
}
