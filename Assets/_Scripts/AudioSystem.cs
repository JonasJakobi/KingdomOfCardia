using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Basic audio system that can play music and sound effects.
/// </summary>
public class AudioSystem : SingletonPersistent<AudioSystem>
{
    [SerializeField] public float originalVolume;
    [SerializeField] public float originalPitch;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buildMusic;
    [SerializeField] private AudioClip menuClick;
    [SerializeField] private AudioClip basicClick;
    [SerializeField] private AudioClip bonkSound;
    [SerializeField] private AudioClip basicSplash;

    void Start()
    {
        originalPitch = 0.8f;
        originalVolume = 0.2f;
        musicSource.volume = originalVolume;
        musicSource.pitch = originalPitch;
    }

    public void ChangePitch(float pitch)
    {
        musicSource.pitch = pitch;
    }

    public IEnumerator FadeOutMusic(float fadeOutTime, AudioClip newMusic, bool randomPitch)
    {
        float duration = fadeOutTime; // Duration of the fade
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume -= Time.deltaTime / fadeOutTime;

            yield return null;
        }
        musicSource.volume = originalVolume;
        PlayMusic(newMusic, randomPitch);
    }

    public void PlayMusic(AudioClip clip, bool randomPitch)
    {
        if (randomPitch)
        {
            float randVal = Random.Range(0.6f, 1.2f);
            musicSource.pitch = randVal;
            originalPitch = randVal;
        }
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayBackgroundMusic()
    {
        StartCoroutine(FadeOutMusic(1f, backgroundMusic, true));
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic, false);
    }

    public void PlayBuildMusic()
    {
        StartCoroutine(FadeOutMusic(1f, buildMusic, true));
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
