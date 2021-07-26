using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePieceUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
#pragma warning disable 0649

    [SerializeField] private PuzzlePiece link;
    private Image img;

#pragma warning restore 0649


    public void Init(PuzzlePiece p)
    {
        link = p;
        img = GetComponent<Image>();
        img.sprite = p.Sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        img.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGING...");
        link.StartSnapppingToPointer();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        img.enabled = true;
        link.EndSnapppingToPointer();
    }
}