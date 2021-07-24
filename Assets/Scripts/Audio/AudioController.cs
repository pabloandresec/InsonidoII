using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioClip[] sfxClips;
    public AudioClip[] musicClips;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    [Header("General")]
    public bool soundEnabled = true;
    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;

    public int loopedIndex = -1;

    private bool courutine = false;

    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(sfxClips[index]);
    }
    public void StartSFXPlayLooped(int index)
    {
        if (loopedIndex == index) return;
        loopedIndex = index;
        sfxSource.clip = sfxClips[index];
        sfxSource.loop = true;
        sfxSource.Play();
    }
    public void StopSFXPlayLooped(int index)
    {
        if (sfxSource.clip != sfxClips[index]) return;
        loopedIndex = -1;
        sfxSource.Stop();
        sfxSource.clip = null;
        sfxSource.loop = false;
    }
    public void PlaySFX(AudioClip clip)
    {
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
