using api;
using System;
using System.Collections;
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

    [Header("User Input")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField codeInput;
    
    [SerializeField] private Button nameReloadButton;
    [SerializeField] private Button nameProceedButton;
    [SerializeField] private Button hostGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button playButton;



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
    [SerializeField] private int menuSceneIndex;

    public static bool CanJoin => GameManager.myName.Length > 0 && Client.IsConnected && !namePending;

   
    private static bool _startPending = false;
    private static bool namePending = false;

    private void Start()
    {
        StartJoinServer();
        SetNamePending();

        nameInput.onValueChanged.AddListener(SetNamePending);
        nameReloadButton.onClick.AddListener(ApplyName);
        hostGameButton.onClick.AddListener(HostGame);
        joinGameButton.onClick.AddListener(JoinGame);
    }

    void Update()
    {
        CheckPromises();
        CheckConnection();
        CheckStartGame();

        nameReloadButton.interactable = nameInput.text.Length > 0;
    }

    private void JoinGameScene()
    {
        SceneManager.LoadScene(gameSceneIndex, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(menuSceneIndex);
    }

    private void StartJoinServer()
    {
        if (groundClient)
            Debug.LogWarning("Client is grounded, it will not attempt to connect to the server");
        else
            Client.JoinServer();
    }

    private void CheckPromises()
    {
        CheckNamePromise();
        CheckJoinPromise();
        CheckHostPromise();
    }

    private void CheckStartGame()
    {
        if (Methods.IsGameActive)
        {
            if (_startPending)
                JoinGameScene();
            //else //If the client does not want to start don't make them
            //    throw new UnexpectedPacketException();//A game flagging as running when I am not waiting is unexpected
        }
    }

    private void CheckConnection()
    {
        bool isConnected = groundClient || Client.IsConnected;

        //set server connection indicator's status
        //TODO: make the user get sent to the main menu when the server disconnects for whatever reason
        SetStatus(serverConnectionIndicator, isConnected ? connectionStatusSuccess : connectionStatusFailure);
        if(!isConnected && UICarousel.TargetPosition != 0)
            UICarousel.TargetPosition = 0;

        playButton.enabled = isConnected;
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

                _startPending = true;
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

                _startPending = true;
            }
            else
            {
                //failure
                throw new UnexpectedPacketException();
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

        backButton.onBackCalls.Enqueue(ExitLimbo);
    }

    private void ExitLimbo()
    {
        Methods.LeaveGame();
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
}
