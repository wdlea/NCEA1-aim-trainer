using api;
using api.Plugins;
using System;
using System.Collections;
using System.Threading.Tasks;
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
    Task<string>? namePromise;
    Task<bool>? joinPromise;
    Task<string>? hostPromise;

    [Header("User Input")]

    #pragma warning disable CS8618//they are filled out in the editor
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

    #pragma warning restore CS8618
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
            Client.StartClient();
    }

    #region statusUpdates
    private void CheckPromises()
    {
        CheckNamePromise();
        CheckJoinPromise();
        CheckHostPromise();
    }

    private void CheckStartGame()
    {
        if (Broadcasts.IsGameActive)
        {
            if (_startPending)
                JoinGameScene();
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

    #endregion statusUpdates

    #region promiseChecking
    private void CheckHostPromise()
    {
        if (hostPromise is not null && hostPromise.IsCompleted)
        {
            try
            {
                string result = hostPromise.GetAwaiter().GetResult();
                joinCodeText.gameObject.SetActive(true);
                joinCodeText.text = result;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }

    private void CheckJoinPromise()
    {
        if (joinPromise is not null && joinPromise.IsCompleted)
        {
            try
            {
                joinPromise.GetAwaiter().GetResult();

                joinCodeText.gameObject.SetActive(false);
            }catch(Exception e)
            {
                throw e;
            }
        }
    }

    private void CheckNamePromise()
    {
        if (namePromise is not null && namePromise.IsCompleted)
        {
            try
            {
                string newName = namePromise.GetAwaiter().GetResult();

                GameManager.myName = newName;

                SetStatus(nameStatusIndicator, nameStatusCompleted);
                SetStatus(nameReloadButton, nameProceedButton, nameStatusCompleted);
            }catch(Exception e)
            {
                SetStatus(nameStatusIndicator, nameStatusError);
                SetStatus(nameReloadButton, nameProceedButton, nameStatusError);

                throw e;
            }
            finally
            {
                namePromise = null;
            }
        }
    }

    #endregion promiseChecking

    #region buttonMethods
    private void ApplyName()
    {
        string name = nameInput.text;

        if (groundClient)
        {
            TaskCompletionSource<string> source = new();
            namePromise = source.Task;

            source.SetResult(name);
        }
        else
            namePromise = Methods.SetName(name);
        

        SetStatus(nameStatusIndicator, nameStatusAwaiting);
        SetStatus(nameReloadButton, nameProceedButton, nameStatusAwaiting);

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

    #endregion buttonMethods

    #region limbo
    private void EnterLimbo()
    {
        UICarousel.TargetPosition = limboCarouselPosition;

        _startPending = true;

        backButton.onBackCalls.Enqueue(ExitLimbo);
    }

    private void ExitLimbo()
    {
        _startPending = false;
        #pragma warning disable CS4014 
        //I don't really care when this completes, the server is written to handle the 
        //packets in series, so this will complete before the next packet is handled.
        Methods.LeaveGame();
        #pragma warning restore CS4014
    }

    #endregion limbo

    #region statusIndicators
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

    #endregion statusIndicators
}
