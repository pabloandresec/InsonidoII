using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Puzzle : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private TextMeshProUGUI DebugText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private static bool debug;
    private static TextMeshProUGUI uguiRef;

#pragma warning restore 0649


    private void Awake()
    {
        uguiRef = DebugText;
        StartPuzzle();
    }

    public virtual void RefreshPuzzle()
    {

    }
    public virtual void StartPuzzle()
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
    public void ResizeSpriteToScreen()
    {
        if (spriteRenderer == null) return;
        //Set vars
        Vector3 camPos = Camera.main.transform.position;
        spriteRenderer.transform.localScale = new Vector3(1, 1, 1);

        //Get Sizes
        float width = spriteRenderer.sprite.bounds.size.x;
        float height = spriteRenderer.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        //Set values
        spriteRenderer.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        spriteRenderer.transform.position = new Vector3(camPos.x, camPos.y, 0);
    }
}
