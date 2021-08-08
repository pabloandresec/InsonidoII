using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool keepAspectRatio = false;
    [SerializeField] private SpriteRenderer mainRenderer = null;
    [SerializeField] private UnityEvent onCutsceneFinish;

    bool playing = false;
    double exitTime;

    private void Start()
    {
        ScaleSprite(keepAspectRatio, mainRenderer, Camera.main);
        Play();
        CalculateExitTime();
    }

    public void Play()
    {
        director.Play();
        playing = true;
    }

    public void Stop()
    {
        director.Stop();
        playing = false;
    }

    private void CalculateExitTime()
    {
        exitTime = director.duration - 0.01f;
    }

    private void Update()
    {
        if(playing)
        {
            if (director.time >= exitTime)
            {
                playing = false;
                onCutsceneFinish?.Invoke();
            }
        }
    }


    public void ScaleSprite(bool keepAspecRatio, SpriteRenderer sprite, Camera cam)
    {
        Vector3 topRightCorner = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));//Calcular esquina superior de la camara

        float worldSpaceWidth = topRightCorner.x * 2;
        float worldSpaceHeight = topRightCorner.y * 2;

        Vector2 spriteSize = sprite.bounds.size;

        float scaleFactorX = worldSpaceWidth / spriteSize.x;
        float scaleFactorY = worldSpaceHeight / spriteSize.y;

        if (keepAspecRatio)
        {
            if (scaleFactorX > scaleFactorY)
            {
                scaleFactorY = scaleFactorX;
            }
            else
            {
                scaleFactorX = scaleFactorY;
            }
        }

        sprite.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
    }

    public void LoadScene(int s)
    {
        Debug.Log("Loading Scene " + s);
        SceneManager.LoadSceneAsync(s);
    }
}
