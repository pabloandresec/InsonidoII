using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private Vector3 ogPos;
    [SerializeField] private Sprite sprite;
    private bool snappingToPointer;
    private int currentRot = 0;

    public Sprite Sprite { get => sprite; }

#pragma warning restore 0649


    public void InitPiece()
    {
        ogPos = transform.position;
        sprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void Update()
    {
        if(snappingToPointer)
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPos.z = 0;
            transform.position = touchPos;
        }
    }

    public void RotateNext()
    {
        int nextRot = currentRot++;
        if(nextRot > 4)
        {
            nextRot = 0;
        }
        RotatePiece(nextRot);
    }

    public void StartSnapppingToPointer()
    {
        gameObject.SetActive(true);
        snappingToPointer = true;
    }

    public void EndSnapppingToPointer()
    {
        snappingToPointer = false;
        transform.position = new Vector3(-1, -1, 0);
        gameObject.SetActive(false);
    }

    public void RotatePiece(int rot)
    {
        rot = Mathf.Clamp(rot, 0, 4);
        if(rot == currentRot)
        {
            Debug.Log("Already in that rotation!");
        }
        switch(rot)
        {
            case 0:
                LeanTween.rotateZ(gameObject, 0f ,0.25f);
                currentRot = rot;
                break;
            case 1:
                LeanTween.rotateZ(gameObject, -90f, 0.25f);
                currentRot = rot;
                break;
            case 2:
                LeanTween.rotateZ(gameObject, -180f, 0.25f);
                currentRot = rot;
                break;
            case 3:
                LeanTween.rotateZ(gameObject, -90f, 0.25f);
                currentRot = rot;
                break;
        }
    }
}
