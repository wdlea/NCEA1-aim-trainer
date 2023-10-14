using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Jukebox : MonoBehaviour
{
    [Header("Tracks")]
    [SerializeField] private MusicTrack[] tracks;
    [SerializeField] [ReadOnlyEditor] private int currentTrackIndex = int.MaxValue;//start on a high value so all tracks have equal chance to be played

    [Header("Audio source")]
    [SerializeField] private AudioSource musicSource;

    [Header("UI")]
    [SerializeField] private Button nextTrack;
    [SerializeField] private Button restartTrack;

    [SerializeField] private TMP_Text trackNameText;
    [SerializeField] private TMP_Text trackArtistText;
    [SerializeField] private TMP_Text trackURIText;


    public AudioClip CurrentClip => tracks[currentTrackIndex].audio;
    public string CurrentArtist => tracks[currentTrackIndex].artist;
    public string CurrentName => tracks[currentTrackIndex].trackName;
    public string CurrentURI => tracks[currentTrackIndex].URL;

    public static Jukebox Active { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Active != null && Active != this)
            Debug.LogWarning("More that 1 Jukebox exists in scene at once, audio may be weird");

        Active = this;

        PlayRandom();

        nextTrack.onClick.AddListener(PlayRandom);
        restartTrack.onClick.AddListener(RestartCurrent);
    }

    void Update(){
        if(musicSource.time >= CurrentClip.length)
            PlayRandom();
    }

    void PlayRandom()
    {
        int newIndex = Random.Range(0, tracks.Length - 1);
        if (newIndex >= currentTrackIndex)
            newIndex++;//shift up to avoid double playing a track

        currentTrackIndex = newIndex;

        SetUI();
        RestartCurrent();
    }

    void RestartCurrent()
    {
        musicSource.clip = CurrentClip;
        musicSource.time = 0;
        musicSource.Play();
    }

    void SetUI()
    {
        trackNameText.text = CurrentName;
        trackArtistText.text = CurrentArtist;
        trackURIText.text = CurrentURI;
    }
}
