using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Music Track", menuName ="Music Track")]
public class MusicTrack : ScriptableObject
{
    public AudioClip audio;
    public string artist;
    public string trackName;
    public string URL;
}
