using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicPuzzle : Puzzle
{
#pragma warning disable 0649

    [SerializeField] private Transform piecesParent;
    [SerializeField] private RectTransform piecesHolder;
    [SerializeField] private GameObject imagePrefab;

#pragma warning restore 0649
    
    private PuzzlePiece piece;

    public override void StartPuzzle()
    {
        base.StartPuzzle();

        for (int i = 0; i < piecesParent.childCount; i++)
        {
            //Init active piece
            PuzzlePiece p = piecesParent.GetChild(i).GetComponent<PuzzlePiece>();
            p.InitPiece();

            //Instantiate ui piece
            GameObject img = Instantiate(imagePrefab, piecesHolder, false);
            img.GetComponent<PuzzlePieceUI>().Init(p);

            // Disable active piece
            p.transform.position = new Vector3(-1, -1, 0);
            p.gameObject.SetActive(false);
        }
    }
}
