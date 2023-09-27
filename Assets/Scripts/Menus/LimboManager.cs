using api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LimboManager : MonoBehaviour
{
    [SerializeField] private int gameSceneIndex;
    [SerializeField] private int menuSceneIndex;

    [SerializeField] private Button quitButton;

    [SerializeField] private Text codeText;

    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitLoading);
    }

    // Update is called once per frame
    void Update()
    {
        if (Methods.IsHost)
        {
            codeText.text = Methods.GameCode ?? "-";//display - until code has been found
        }
        else
        {
            codeText.text = "joining";
        }

        if (Methods.IsGameRunning)
        {
            SceneManager.LoadScene(gameSceneIndex);
        }
    }

    /// <summary>
    /// Quits waiting for the game to load
    /// </summary>
    void QuitLoading()
    {
        Methods.LeaveGame();
        SceneManager.LoadScene(menuSceneIndex);
    }
}
