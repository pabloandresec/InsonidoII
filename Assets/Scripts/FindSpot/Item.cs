using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IDragHandler, IBeginDragHandler , IEndDragHandler
{
#pragma warning disable 0649

    [SerializeField] private VibrationWave wave;
    [SerializeField] private Transform tgtParent;
    private Transform ogParent;
    bool dragged = false;
    bool draggable = true;
    RectTransform myRect;

#pragma warning restore 0649


    private void Start()
    {
        ogParent = transform.parent;
        Puzzle.SetMobileDebugState(false);
    }

    public void SetTGT(Transform tgt)
    {
        tgtParent = tgt;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!draggable)
        {
            Puzzle.MobileDebug("Is not draggble");
            wave.SetVibration(false);
            return;
        }
        Puzzle.MobileDebug("Started Dragging");
        wave.SetVibration(true);
        dragged = true;
        transform.SetParent(tgtParent.parent.parent);
        transform.SetAsLastSibling();
        myRect = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragged)
        {
            //Puzzle.MobileDebug("Dragging");
            //Debug.Log("Dragging " + transform.name);
            Vector3 newPos = eventData.position;
            myRect.position = newPos;
            float dist = Vector2.Distance(myRect.position, tgtParent.position);
            wave.SetVibrationAmount(Mathf.InverseLerp(200, 0, dist));
            //Debug.Log("Dist to target = " + dist);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Puzzle.MobileDebug("lifted pointer");
        if (dragged)
        {
            StartCoroutine(CheckPlace(eventData));
        }
        wave.SetVibration(false);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<FindSpot>().CheckForCompletition();
    }

    private IEnumerator CheckPlace(PointerEventData eventData)
    {
        Puzzle.MobileDebug("End Draggin... Checking for pos " + Input.mousePosition);
        bool correct = false;
        for (int i = 0; i < tgtParent.parent.childCount; i++)
        {
            RectTransform g = tgtParent.parent.GetChild(i).GetComponent<RectTransform>();
            Puzzle.MobileDebug("<#FFFFFF>Testing -> <#FF0085>" + g.transform.name + g.rect);

            Vector2 localMousePosition = g.InverseTransformPoint(Input.mousePosition);
            if (g.rect.Contains(localMousePosition))
            {
                if(g.transform == tgtParent)
                {
                    Puzzle.MobileDebug("<#2EFF00>" + g.transform.name + " was the correct slot<#FFFFFF>");
                    correct = true;
                    break;
                }
            }
            yield return null;
        }
        Puzzle.MobileDebug("<#FFFFFF>");
        if (correct)
        {
            Puzzle.MobileDebug("Piece placed");
            Debug.Log("Piece placed");
            /*
            if(FindSpot.android)
            {
                Handheld.Vibrate();
            }
            */
            transform.SetParent(tgtParent);
            myRect.position = tgtParent.GetComponent<RectTransform>().position;
            draggable = false;
        }
        else
        {
            Puzzle.MobileDebug("Wrong place");
            transform.SetParent(ogParent);
        }
        transform.GetComponent<Image>().enabled = true;
        dragged = false;
    }
}
