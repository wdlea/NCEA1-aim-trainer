using api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable enable

/// <summary>
/// Manages the main menu of the game
/// </summary>
public class MenuManager : MonoBehaviour
{
    Promise<string>? namePromise;
    Promise<bool>? joinPromise;
    Promise<string>? hostPromise;

    [SerializeField] Surrogate surrogate;

    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField codeInput;

    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;

    [SerializeField] private Indicator nameIndicator;

    [SerializeField] private int limboSceneIndex;

    public static bool CanJoin => GameManager.myName.Length > 0 && Client.IsConnected && !namePending;
    static bool namePending = false;

    private void Start()
    {
        nameInput.onEndEdit.AddListener(ApplyName);
        joinButton.onClick.AddListener(JoinGame);
        hostButton.onClick.AddListener(HostGame);
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
            else if (playerName is not null)
            {
                GameManager.myName = playerName;
                nameIndicator.State = Indicator.IndicatorState.Completed;
            }
            namePending = false;

            namePromise = null;
        }

        if(joinPromise != null && joinPromise.Finished)
        {
            if(joinPromise.Get(out bool success) is Exception e)
            {
                throw e;
            }
            else if(success)
            {
                SceneManager.LoadScene(limboSceneIndex);
            }
            else
            {
                throw new Exception("Achievement unlocked: How did we get here?");//undefined behavior
            }
            joinPromise = null;
        }

        if(hostPromise != null && hostPromise.Finished)
        {
            if (hostPromise.Get(out string? code) is Exception e)
            {
                throw e;
            }
            else if(code is not null)
            {
                SceneManager.LoadScene(limboSceneIndex);
                Debug.Log(code);
            }
            else
            {
                throw new Exception("Achievement unlocked: How did we get here?");//undefined behavior
            }
            hostPromise = null;
        }
    }

    public void ApplyName(string name)
    {
        namePromise = Methods.SetName(name);
        nameIndicator.State = Indicator.IndicatorState.Pending;
        namePending = true;
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
