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
    [SerializeField] private AudioClip menuClick;
    [SerializeField] private AudioClip basicClick;
    [SerializeField] private AudioClip bonkSound;
    [SerializeField] private AudioClip basicSplash;

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

    public void PlayMenuClickSound()
    {
        PlaySound(menuClick);
    }

    public void PlayClickSound()
    {
        PlaySound(basicClick);
    }

    public void PlayBonkSound()
    {
        PlaySound(bonkSound);
    }
    public void PlaySplash()
    {
        PlaySound(basicSplash);
    }
}
