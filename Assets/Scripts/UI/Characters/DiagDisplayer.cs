using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using System;

public abstract class DiagDisplayer : MonoBehaviour
{
    [Header("--------------------Settings---------------")]
    [SerializeField] protected bool startOnStartLevel;
    [Min(0.05f)]
    [SerializeField] protected float charTime = 0.09f;
    [Min(0.05f)]
    [SerializeField] protected float betweenLinesTime = 0.09f;
    [SerializeField] protected RectTransform[] positions;

    [Header("------------------Cutscene------------------")]
    [SerializeField] protected DialogPack[] cutsceneDialogs;
    [Header("------------------EVENTOS------------------")]
    [SerializeField] protected UnityEvent onComplete;

    protected bool skipped = false;
    protected int currentDialog = 0;

    protected void HideCharacter(string transformName)
    {
        Debug.Log("Hiding " + transformName);
        RectTransform t = positions.FirstOrDefault(p => p.transform.name == transformName);
        Image img = t.gameObject.GetComponent<Image>();
        if (t == null)
        {
            Debug.Log("Position " + transformName + " not found");
            return;
        }
        LeanTween.scale(t, new Vector3(0.75f, 0.75f, 0.75f), 0.25f);

        if (img.color != new Color(0.25f, 0.25f, 0.25f))
        {
            LeanTween.value(t.gameObject, (c) => {
                img.color = c;
            }, new Color(1f, 1f, 1f), new Color(0.25f, 0.25f, 0.25f), 0.25f);
        }
    }

    protected void ShowCharacter(string transformName)
    {
        Debug.Log("Showing " + transformName);
        RectTransform t = positions.FirstOrDefault(p => p.transform.name == transformName);
        Image img = t.gameObject.GetComponent<Image>();
        if (t == null)
        {
            Debug.Log("Position " + transformName + " not found");
            return;
        }
        LeanTween.scale(t, new Vector3(1f, 1f, 1f), 0.25f);
        if (img.color != new Color(1f, 1f, 1f))
        {
            LeanTween.value(t.gameObject, (c) => {
                img.color = c;
            }, new Color(0.25f, 0.25f, 0.25f), new Color(1f, 1f, 1f), 0.25f);
        }
    }

    protected void HideCharacter(RectTransform rt)
    {
        //Debug.Log("Hiding " + rt.transform.name);
        Image img = rt.gameObject.GetComponent<Image>();
        if (rt == null)
        {
            Debug.Log("Position " + rt.transform.name + " not found");
            return;
        }
        LeanTween.scale(rt, new Vector3(0.75f, 0.75f, 0.75f), 0.25f);
        if (img.color != new Color(0.25f, 0.25f, 0.25f))
        {
            LeanTween.value(rt.gameObject, (c) => {
                img.color = c;
            }, new Color(1f, 1f, 1f), new Color(0.25f, 0.25f, 0.25f), 0.25f);
        }
    }

    protected void ShowCharacter(RectTransform rt, Action onCharacterVisible)
    {
        //Debug.Log("Showing " + rt.transform.name);
        Image img = rt.gameObject.GetComponent<Image>();
        if (rt == null)
        {
            Debug.Log("Position " + rt.transform.name + " not found");
            return;
        }
        LeanTween.scale(rt, new Vector3(1f, 1f, 1f), 0.25f);
        if (img.color != new Color(1f, 1f, 1f))
        {
            LeanTween.value(rt.gameObject, (c) => {
                img.color = c;
            }, new Color(0.25f, 0.25f, 0.25f), new Color(1f, 1f, 1f), 0.25f).setOnComplete(() => {
                onCharacterVisible?.Invoke();
            });
        }
    }

    public virtual void SkipText()
    {
        Debug.Log("Skipping!");
    }

    protected virtual void PrepareAndStart()
    {
        Debug.Log("Preparando los dialogos");
    }

    protected virtual void StartDialogCutscene(int index)
    {
        Debug.Log("Comenzando");
    }
}
