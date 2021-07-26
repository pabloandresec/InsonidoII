using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPiece : MonoBehaviour
{
    [SerializeField] private Vector2Int tGTPos;

    public Vector2Int TGTPos
    {
        get { return tGTPos; }
        set { tGTPos = value; }
    }
}
