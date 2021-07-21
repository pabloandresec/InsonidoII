using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Puzzle : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private TextMeshProUGUI DebugText;

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
}
