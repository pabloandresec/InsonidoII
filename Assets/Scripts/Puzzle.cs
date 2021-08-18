using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using System;

public abstract class Puzzle : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private TextMeshProUGUI DebugText;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer tutorial;

    private static bool debug;
    private static TextMeshProUGUI uguiRef;
    protected bool paused = false;

#pragma warning restore 0649


    private void Awake()
    {
        uguiRef = DebugText;
        StartPuzzle();
    }

    public virtual void PauseGame()
    {

    }

    public virtual void ResumeGame()
    {

    }

    public virtual void RestartGame()
    {

    }

    public virtual void RefreshPuzzle()
    {

    }
    public virtual void StartPuzzle()
    {

    }

    public virtual void ShowTutorial(bool show)
    {

    }

    public static void SetMobileDebugState(bool state)
    {
        debug = state;
    }
    public static void MobileDebug(string text)
    {
        if(!debug)
        {
            return;
        }
        uguiRef.text += "\n" + text;
    }
    public void SetBackgroundTransform(Vector3 pos, int size)
    {
        spriteRenderer.transform.position = pos;
        spriteRenderer.transform.localScale = new Vector3(size, size, 1);
        Debug.Log("Background Transform Updated");
    }

    

    public void ResizeSpriteToScreen(Camera cam, SpriteRenderer sprite)
    {
        Debug.Log("Resizing Sprite!");
        if (sprite == null) return;
        //Set vars
        Vector3 camPos = cam.transform.position;
        sprite.transform.localScale = new Vector3(1, 1, 1);

        //Get Sizes
        float width = sprite.sprite.bounds.size.x;
        float height = sprite.sprite.bounds.size.y;

        float worldScreenHeight = cam.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        //Set values
        sprite.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        sprite.transform.position = new Vector3(camPos.x, camPos.y, 0);
    }
    public void ResizeSpriteToScreen(CinemachineVirtualCamera cam, SpriteRenderer sprite)
    {
        Debug.Log("Resizing Sprite!");
        if (sprite == null) return;
        //Set vars
        Vector3 camPos = cam.transform.position;
        sprite.transform.localScale = new Vector3(1, 1, 1);

        //Get Sizes
        float width = sprite.sprite.bounds.size.x;
        float height = sprite.sprite.bounds.size.y;

        float worldScreenHeight = cam.m_Lens.OrthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        //Set values
        sprite.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        sprite.transform.position = new Vector3(camPos.x, camPos.y, 0);
    }
}
