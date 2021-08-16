using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [Header("General")]
    [SerializeField]
    [Tooltip("Si es menor a 0 no funciona")]
    private int playMusicOnAwake = -1;
    [SerializeField] private bool soundEnabled = true;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private AudioMixer musicMixer;

    [SerializeField] private int loopedIndex = -1;

    private bool courutine = false;


    private void Start()
    {
        if(playMusicOnAwake >= 0)
        {
            musicSource.clip = musicClips[playMusicOnAwake];
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip clip, float pitch)
    {
        if (clip == null)
        {
            return;
        }
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }

    public void SwapMusic(int index)
    {
        musicSource.Stop();
        musicSource.clip = musicClips[index];
        musicSource.Play();
    }
    public void SwapMusic(int index, FadeMode mode, float fadeTime)
    {
        if (!courutine)
        {
            StartCoroutine(Fade(index, mode, fadeTime));
        }
    }
    public void SimpleSwapMusic(int index)
    {
        if (!courutine)
        {
            StartCoroutine(Fade(index, FadeMode.FADE_IN_AND_OUT, 1f));
        }
    }

    IEnumerator Fade(int index, FadeMode mode, float totalTime)
    {
        courutine = true;
        bool done = false;
        float individualTime = totalTime / 2;
        float tPassed = 0;
        float currentVol = musicSource.volume;

        if (mode == FadeMode.FADE_IN_AND_OUT || mode == FadeMode.FADE_OUT) // Bajar el volumen
        {
            while (!done)
            {
                if (musicSource.volume <= 0.01)
                {
                    done = true;
                }
                tPassed += Time.deltaTime;
                float t = tPassed / individualTime;
                musicSource.volume = Mathf.Lerp(currentVol, 0, t);
                yield return null;
            }
        }

        musicSource.Stop();
        musicSource.clip = musicClips[index];
        musicSource.Play();

        if (mode == FadeMode.FADE_IN_AND_OUT || mode == FadeMode.FADE_IN) // Bajar el volumen
        {
            tPassed = 0;
            done = false;
            while (!done)
            {
                if (musicSource.volume >= currentVol)
                {
                    done = true;
                }
                tPassed += Time.deltaTime;
                float t = tPassed / individualTime;
                musicSource.volume = Mathf.Lerp(0, currentVol, t);
                yield return null;
            }
        }
        else
        {
            musicSource.volume = currentVol;
        }
        courutine = false;
    }
}
public enum FadeMode
{
    FADE_IN_AND_OUT,
    FADE_IN,
    FADE_OUT
}
