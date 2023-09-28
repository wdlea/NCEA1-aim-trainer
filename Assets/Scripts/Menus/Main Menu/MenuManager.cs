using api;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable enable

/// <summary>
/// Manages the main menu of the game
/// </summary>
public class MenuManager : MonoBehaviour, IPreprocessBuildWithReport 
{
    Promise<string>? namePromise;
    Promise<bool>? joinPromise;
    Promise<string>? hostPromise;

    [Header("User Input")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField codeInput;
    
    [SerializeField] private Button nameReloadButton;
    [SerializeField] private Button nameProceedButton;
    [SerializeField] private Button hostGameButton;
    [SerializeField] private Button joinGameButton;

    [Header("Displays")]
    [SerializeField] private Image serverConnectionIndicator;
    [SerializeField] private Image nameStatusIndicator;

    [SerializeField] private TMP_Text joinCodeText;

    
    [Serializable] struct IndicatorStatus
    {
        public Color statusColour;
        public Sprite statusIcon;
        public bool reloadAllowed;
        public bool proceedAllowed;
    }

    [Header("Indicator Statuses")]
    [SerializeField] private IndicatorStatus nameStatusPending;
    [SerializeField] private IndicatorStatus nameStatusAwaiting;
    [SerializeField] private IndicatorStatus nameStatusError;
    [SerializeField] private IndicatorStatus nameStatusCompleted;

    [SerializeField] private IndicatorStatus connectionStatusFailure;
    [SerializeField] private IndicatorStatus connectionStatusSuccess;

    [Header("Developer Utility")]
    //allows me to stop the client from attempting to connect
    [SerializeField] private bool groundClient = false;

    [Header("UI")]
    [SerializeField] private BackButton backButton;
    [SerializeField] private Carousel UICarousel;

    [SerializeField] private float limboCarouselPosition;

    [Header("Scenes")]
    [SerializeField] private int gameSceneIndex;

    private bool waitingForStart = false;

    public static bool CanJoin => GameManager.myName.Length > 0 && Client.IsConnected && !namePending;

    public int callbackOrder => throw new NotImplementedException();

    static bool namePending = false;

    AsyncOperation gameScene;

    private void Start()
    {
        StartJoinServer();
        SetNamePending();
        StartLoadGame();

        nameInput.onValueChanged.AddListener(SetNamePending);
        nameReloadButton.onClick.AddListener(ApplyName);
        hostGameButton.onClick.AddListener(HostGame);
        joinGameButton.onClick.AddListener(JoinGame);
    }

    void Update()
    {
        CheckPromises();
        SetServerStatusIndicator();
        CheckStartGame();
    }

    private void StartLoadGame()
    {
        gameScene = SceneManager.LoadSceneAsync(gameSceneIndex);
        gameScene.allowSceneActivation = false;
    }

    private void JoinGameScene()
    {
        gameScene.allowSceneActivation = true;//join ASAP
        gameScene.allowSceneActivation = true;//join ASAP
    }

    private void StartJoinServer()
    {
        if (groundClient)
            Debug.LogWarning("Client is grounded, it will not attempt to connect to the server");
        else
            StartCoroutine(nameof(AttemptJoinCoroutine));
    }

    private void CheckPromises()
    {
        CheckNamePromise();
        CheckJoinPromise();
        CheckHostPromise();
    }

    private void CheckStartGame()
    {
        if (Methods.IsGameRunning)
        {
            if (waitingForStart)
                JoinGameScene();
            else
                throw new UnexpectedPacketException();//A game flagging as running when I am not waiting is unexpected
        }
    }

    private void SetServerStatusIndicator()
    {
        //set server connection indicator's status
        //TODO: make the user get sent to the main menu when the server disconnects for whatever reason
        SetStatus(serverConnectionIndicator, groundClient || Client.IsConnected ? connectionStatusSuccess : connectionStatusFailure);
    }

    private void CheckHostPromise()
    {
        if (hostPromise is not null && hostPromise.Finished)
        {
            if (hostPromise.Get(out string? code) is null)
            {
                //successful host
                joinCodeText.gameObject.SetActive(true);
                joinCodeText.text = code;
            }
            else
            {
                //failure, undefined behavior so throw error
                throw new UnexpectedPacketException();
            }
        }
    }

    private void CheckJoinPromise()
    {
        if (joinPromise is not null && joinPromise.Finished)
        {
            if (joinPromise.Get(out bool success) is null && success)
            {
                //successful join
                joinCodeText.gameObject.SetActive(false);
            }
            else
            {
                //failure
            }
        }
    }

    private void CheckNamePromise()
    {
        if (namePromise is not null && namePromise.Finished)
        {
            if (namePromise.Get(out string? newName) is null && newName != null)
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
    }

    private void ApplyName()
    {
        string name = nameInput.text;
        if (name.Length <= 0)
            return;

        if (groundClient)
        {
            namePromise = new Promise<string>();
            namePromise.Fulfil(name);
        }
        else
        {
            namePromise = Methods.SetName(name);
        }

        SetStatus(nameStatusIndicator, nameStatusPending);
        SetStatus(nameReloadButton, nameProceedButton, nameStatusPending);

        namePending = true;
    }

    private void HostGame()
    {
        if (groundClient)
            throw new InvalidOperationException("Cannot host a game when you have grounded the client");

        joinCodeText.gameObject.SetActive(true);
        joinCodeText.text = "Loading Code";

        EnterLimbo();

        hostPromise = Methods.CreateGame();
    }

    private void JoinGame()
    {
        if (groundClient)
            throw new InvalidOperationException("Cannot join a game when you have grounded the client");

        joinCodeText.gameObject.SetActive(false);

        EnterLimbo();

        //force lowercase becasue server only accepts lowercase characters
        joinPromise = Methods.JoinGame(codeInput.text.ToLower());
    }

    private void EnterLimbo()
    {
        UICarousel.TargetPosition = limboCarouselPosition;

        waitingForStart = true;
        backButton.onBackCalls.Enqueue(ExitLimbo);
    }

    private void ExitLimbo()
    {
        Methods.LeaveGame();
        waitingForStart = false;
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

    public void OnPreprocessBuild(BuildReport report)
    {
        //Make sure that the server is not grounded when I build it to save my sanity
        groundClient = false;
    }
}
