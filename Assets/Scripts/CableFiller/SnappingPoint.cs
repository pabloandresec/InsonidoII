using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class SnappingPoint : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private int segIndex;
    private CircleCollider2D col;
    private TextMeshPro text;

    public int SegIndex { get => segIndex; }
#pragma warning restore 0649

    public void SetCollider(bool v)
    {
        col.enabled = v;
    }

    public void Init(int i)
    {
        col = GetComponent<CircleCollider2D>();
        text = GetComponent<TextMeshPro>();
        SetSegmentIndex(i);
    }

    public void SetSegmentIndex (int index)
    {
        segIndex = index;
        text.text = index.ToString();
    }
}
