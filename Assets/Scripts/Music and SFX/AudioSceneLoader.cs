using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSceneLoader : MonoBehaviour
{
    [SerializeField] private int audioSceneIndex;

    private void Start()
    {
        if (Jukebox.Active == null)
            LoadAudioScene();
    }

    private void LoadAudioScene()
    {
        SceneManager.LoadScene(audioSceneIndex, LoadSceneMode.Additive);
    }
}
