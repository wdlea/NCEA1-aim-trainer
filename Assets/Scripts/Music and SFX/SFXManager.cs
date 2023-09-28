using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Serializable] struct SFXSound
    {
        public AudioClip clip;
        public AudioSource source;

        public void Play()
        {
            if (source.isPlaying)
                return;

            source.clip = clip;
            source.Play();
        }
    }

    [SerializeField] SFXSound shot;
    [SerializeField] SFXSound click;

    public static SFXManager Active { get; private set; }

    private void Awake()
    {
        if (Active != null && Active != this)
            Debug.LogWarning("More than 1 SFXManager in scene");

        Active = this;
    }

    public void PlayShot() => shot.Play();
    public void PlayClick() => click.Play();
}
