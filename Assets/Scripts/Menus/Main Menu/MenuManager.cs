using api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private TMP_InputField nameInput;
    
    [SerializeField] private Button nameReloadButton;
    [SerializeField] private Button nameProceedButton;

    [SerializeField] private Image serverConnectionIndicator;
    [SerializeField] private Image nameStatusIndicator;

    [Serializable] struct IndicatorStatus
    {
        public Color statusColour;
        public Sprite statusIcon;
        public bool reloadAllowed;
        public bool proceedAllowed;
    }
    [SerializeField] private IndicatorStatus nameStatusPending;
    [SerializeField] private IndicatorStatus nameStatusAwaiting;
    [SerializeField] private IndicatorStatus nameStatusError;
    [SerializeField] private IndicatorStatus nameStatusCompleted;

    [SerializeField] private IndicatorStatus connectionStatusFailure;
    [SerializeField] private IndicatorStatus connectionStatusSuccess;


    [SerializeField] private int limboSceneIndex;

    public static bool CanJoin => GameManager.myName.Length > 0 && Client.IsConnected && !namePending;
    static bool namePending = false;

    private void Start()
    {
        StartCoroutine(nameof(AttemptJoinCoroutine));

        SetNamePending();

        nameInput.onValueChanged.AddListener(SetNamePending);

        nameReloadButton.onClick.AddListener(ApplyName);
    }

    // Update is called once per frame
    void Update()
    {
        if(namePromise is not null && namePromise.Finished)
        {
            if(namePromise.Get(out string? newName) is null && newName != null)
            {
                GameManager.myName = newName;

                SetStatus(nameStatusIndicator, nameStatusCompleted);
                SetStatus(nameReloadButton, nameProceedButton, nameStatusCompleted);
            }
            else
            {
                Debug.Log(namePromise.Get(out _));//log error

                SetStatus(nameStatusIndicator, nameStatusError);
                SetStatus(nameReloadButton, nameProceedButton, nameStatusError);
            }

            namePromise = null;
        }


        //set server connection indicator's status
        //TODO: make the user get sent to the main menu when the server disconnects for whatever reason
        SetStatus(serverConnectionIndicator, Client.IsConnected ? connectionStatusSuccess : connectionStatusFailure);
    }

    public void ApplyName()
    {
        string name = nameInput.text;
        if (name.Length <= 0)
            return;

        namePromise = Methods.SetName(name);

        SetStatus(nameStatusIndicator, nameStatusPending);
        SetStatus(nameReloadButton, nameProceedButton, nameStatusPending);

        namePending = true;
    }

    public void HostGame()
    {
        hostPromise = Methods.CreateGame();
    }

    void SetStatus(Image indicator, IndicatorStatus status)
    {
        indicator.sprite = status.statusIcon;
        indicator.color = status.statusColour;
    }
    void SetStatus(Button reload, Button proceed, IndicatorStatus status)
    {
        reload.interactable = status.reloadAllowed;
        proceed.interactable = status.proceedAllowed;
    }
    
    void SetNamePending()
    {
        SetStatus(nameStatusIndicator, nameStatusPending);
        SetStatus(nameReloadButton, nameProceedButton, nameStatusPending);
    }
    void SetNamePending(object o) => SetNamePending();

    IEnumerator AttemptJoinCoroutine()
    {
        while (true)
        {
            try
            {
                Client.JoinServer();
                yield break;
            }
            catch(Exception e)
            {
                Debug.Log("Failed attempted connection: " + e.Message);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
