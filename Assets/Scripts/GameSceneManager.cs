using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private int menuSceneIndex;
    [SerializeField] private int gameSceneIndex;
    private Scene gameScene;
    private Scene menuScene;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);  
    }

    // Update is called once per frame
    void Update()
    {
       if(api.Client.Connected && PlayerManager.Instance.inGame)
        {
            if (menuLoaded)
            {
                ToGame();
            }
        }
        else
        {
            if (!menuLoaded)
            {
                ToMenu();
            }
        }
    }

    bool menuLoaded = false;

    void ToMenu()
    {
        menuLoaded = true;
        SceneManager.LoadScene(menuSceneIndex, LoadSceneMode.Additive);
    }
    void ToGame()
    {
        menuLoaded = false;
        SceneManager.UnloadSceneAsync(menuSceneIndex);
    }
}
