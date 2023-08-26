using api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MenuManager : MonoBehaviour
{
    Promise<string>? namePromise;
    Promise<bool>? joinPromise;
    Promise<string>? hostPromise;

    [SerializeField] surrogate surrogate;

    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField codeInput;

    public static bool CanJoin => GameManager.myName.Length > 0 && Client.IsConnected;

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

        if(namePromise != null && namePromise.Finished)
        {
            if (namePromise.Get(out string? playerName) is Exception e)
            {
                throw e;
            }
            else if(playerName is not null) GameManager.myName = playerName;
        }

        if(joinPromise != null && joinPromise.Finished)
        {
            if(joinPromise.Get(out bool success) is Exception e)
            {
                throw e;
            }
            else if(success)
            {
                //do shit
            }
            else
            {
                throw new Exception("Achievement unlocked: How did we get here?");//undefined behavior
            }
        }

        if(hostPromise != null && hostPromise.Finished)
        {
            if (hostPromise.Get(out string? code) is Exception e)
            {
                throw e;
            }
            else if(code is not null)
            {
                //do shit
            }
            else
            {
                throw new Exception("Achievement unlocked: How did we get here?");//undefined behavior
            }
        }
    }

    public void ApplyName()
    {
        namePromise = Methods.SetName(nameInput.text);
    }
    public void JoinGame()
    {
        joinPromise = Methods.JoinGame(codeInput.text);
    }
    public void HostGame()
    {
        hostPromise = Methods.CreateGame();
    }
}
