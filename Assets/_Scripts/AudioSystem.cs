using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Basic audio system that can play music and sound effects.
/// </summary>
public class AudioSystem : SingletonPersistent<AudioSystem>
{
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void PlaySound(AudioClip clip)
    {
        soundSource.clip = clip;
        soundSource.Play();
    }
}
