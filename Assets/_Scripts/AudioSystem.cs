using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Basic audio system that can play music and sound effects.
/// </summary>
public class AudioSystem : SingletonPersistent<AudioSystem>
{
    [SerializeField] public float originalVolume;
    [SerializeField] public float originalPitch;
    [SerializeField] private List<AudioSource> audioSourceList;
    [SerializeField] private Slider musicSlider, sfxSlider, volumeSlider;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource uiSoundSource;
    [SerializeField] private AudioSource cardSoundSource;
    [SerializeField] private AudioSource cardEffectSoundSource;
    [SerializeField] private AudioSource enemySoundSource;
    [SerializeField] private AudioSource enemySoundSource2;
    [SerializeField] private AudioSource enemySoundSource3;
    [SerializeField] private AudioSource towerSoundSource;
    [SerializeField] private AudioSource projectileSoundSource;
    [SerializeField] private AudioSource anvilSoundSource;
    [SerializeField] private AudioSource dialogueSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buildMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip menuClick;
    [SerializeField] private AudioClip basicClick;
    [SerializeField] private AudioClip bonkSound;
    [SerializeField] private AudioClip basicSplash;
    [SerializeField] private AudioClip dramaticBoom;
    [SerializeField] private AudioClip anvilSound;
    [SerializeField] private AudioClip sellSound;
    [SerializeField] private AudioClip plopp;
    [SerializeField] private AudioClip dialogueSound;

    void Start()
    {
        originalPitch = 0.8f;
        originalVolume = ConsistentSettings.musicVolume;
        musicSource.volume = originalVolume;
        musicSource.pitch = originalPitch;
        audioSourceList.Add(soundSource);
        audioSourceList.Add(uiSoundSource);
        audioSourceList.Add(cardSoundSource);
        audioSourceList.Add(cardEffectSoundSource);
        audioSourceList.Add(enemySoundSource);
        audioSourceList.Add(enemySoundSource2);
        audioSourceList.Add(enemySoundSource3);
        audioSourceList.Add(towerSoundSource);
        audioSourceList.Add(projectileSoundSource);
        audioSourceList.Add(anvilSoundSource);
        audioSourceList.Add(dialogueSource);
        if (sfxSlider != null && musicSlider != null && volumeSlider != null)
        {
            sfxSlider.value = ConsistentSettings.sfxVolume;
            musicSlider.value = ConsistentSettings.musicVolume;
            volumeSlider.value = ConsistentSettings.generalVolume;
            GeneralVolume();
        }
        else
        {
            musicSource.volume = ConsistentSettings.musicVolume;
        }

    }

    public void GeneralVolume()
    {
        SFXVolume();
        MusicVolume();
        ConsistentSettings.generalVolume = volumeSlider.value;
    }

    public void SFXVolume()
    {
        foreach (AudioSource source in audioSourceList)
        {
            source.volume = sfxSlider.value * volumeSlider.value;
        }
        ConsistentSettings.sfxVolume = sfxSlider.value;
    }

    public void MusicVolume()
    {
        musicSource.volume = musicSlider.value * volumeSlider.value;
        originalVolume = musicSlider.value * volumeSlider.value;
        ConsistentSettings.musicVolume = musicSlider.value;
    }
    /// <summary>
    /// Change pitch to new pitch forever
    /// </summary>
    /// <param name="pitch"></param>
    public void ChangePitch(float pitch)
    {
        musicSource.pitch = pitch;

    }
    /// <summary>
    /// Changes pitch and then back to original after duration
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="duration"></param>
    public void ChangePitch(float pitch, float duration)
    {
        ChangePitch(pitch);
        StartCoroutine(ChangePitchBack(duration));
    }
    private IEnumerator ChangePitchBack(float duration)
    {
        yield return new WaitForSeconds(duration);
        ChangePitch(originalPitch);
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

    public void PlayGameOverMusic()
    {
        ChangePitch(1.0f);
        originalVolume = 1.0f;
        StartCoroutine(FadeOutMusic(1f, gameOverMusic, false));
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.clip = clip;
        soundSource.Play();
    }

    public void PlayUISound(AudioClip clip)
    {
        uiSoundSource.clip = clip;
        uiSoundSource.Play();
    }

    public void PlayMenuClickSound()
    {
        PlayUISound(menuClick);
    }

    public void PlayClickSound()
    {
        PlayUISound(basicClick);
    }

    public void PlayBonkSound()
    {
        PlayUISound(bonkSound);
    }
    public void PlaySplash()
    {
        PlaySound(basicSplash);
    }

    public void PlayDramaticBoom()
    {
        PlaySound(dramaticBoom);
    }
    public void PlayAnvilSound(int level)
    {
        anvilSoundSource.clip = anvilSound;
        anvilSoundSource.pitch = 0.65f + (level * 0.05f);
        anvilSoundSource.Play();
    }

    public void PlayPloppSound()
    {
        PlaySound(plopp);
    }

    public void PlayCardSound(AudioClip clip)
    {
        cardSoundSource.clip = clip;
        cardSoundSource.Play();
    }

    public void PlayCardEffectSound(AudioClip clip)
    {
        cardEffectSoundSource.clip = clip;
        cardEffectSoundSource.Play();
    }

    public void PlayEnemySound(AudioClip clip)
    {
        int randVal = Random.Range(1, 4);
        AudioSource selectedSoundSource;

        if (randVal == 1) selectedSoundSource = enemySoundSource;
        else if (randVal == 2) selectedSoundSource = enemySoundSource2;
        else selectedSoundSource = enemySoundSource3;
        selectedSoundSource.pitch = Random.Range(0.6f, 1.2f);
        selectedSoundSource.clip = clip;
        selectedSoundSource.Play();
    }

    public void PlayTowerSound(AudioClip clip)
    {
        towerSoundSource.pitch = Random.Range(0.6f, 1.2f);
        towerSoundSource.clip = clip;
        towerSoundSource.Play();
    }

    public void PlayProjectileSound(AudioClip clip)
    {
        projectileSoundSource.pitch = Random.Range(0.6f, 1.2f);
        projectileSoundSource.clip = clip;
        projectileSoundSource.Play();
    }
    public void PlaySellSound()
    {
        PlaySound(sellSound);
    }

    public void PlayDialogueSound()
    {
        dialogueSource.pitch = Random.Range(0.6f, 1.2f);
        dialogueSource.clip = dialogueSound;
        dialogueSource.Play();
    }

}
