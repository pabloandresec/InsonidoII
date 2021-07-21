using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PieceSlider : Puzzle
{
#pragma warning disable 0649

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2Int centerPieceCoord;
    [SerializeField] private Sprite[] pieces;
    [SerializeField] private Sprite full;
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private float gridBorder = 1f;
    [SerializeField] private LayerMask piecesLayer;
    private SlidingPiece[,] activePieces;
    private Vector2Int emptyTile = Vector2Int.zero;
    private SlidingPiece hiddenPiece;

#pragma warning restore 0649


    public override void StartPuzzle()
    {
        base.StartPuzzle();
        Utils.SetCameraInMiddleOfGrid(piecePrefab.GetComponent<Renderer>(), Camera.main, gridSize, gridBorder);
        SetupGrid();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2Int clickPosition = GetPieceAtPointerPos();
            if (clickPosition != -Vector2Int.one)
            {
                CheckForSlide(clickPosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            activePieces = FixPuzzle();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckForEndGame();
        }
    }

    private SlidingPiece[,] FixPuzzle()
    {
        SlidingPiece[,] fixedTable = new SlidingPiece[gridSize.x,gridSize.y];


        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2Int tgtPos = activePieces[x,y].TGTPos;
                fixedTable[tgtPos.x, tgtPos.y] = activePieces[x, y];
            }
        }

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                fixedTable[x, y].transform.position = new Vector3(x, y, 0);
            }
        }
        return fixedTable;
    }

    private void CheckForSlide(Vector2Int coords)
    {
        bool canSlide = false;
        if(coords.y == emptyTile.y)
        {
            canSlide = true;
        }
        if (coords.x == emptyTile.x)
        {
            canSlide = true;
        }
        if(canSlide)
        {
            Vector2 normDir = ((Vector2)emptyTile - (Vector2)coords).normalized;
            Vector2Int direction = new Vector2Int(Mathf.RoundToInt(normDir.x), Mathf.RoundToInt(normDir.y));
            Vector2Int ogPos = coords;
            //Debug.Log("Clicked "+ ogPos + " Direction " + direction);
            Vector2Int workingTile = emptyTile - direction;
            int emergencyCounter = 0;
            bool samePos = false;
            while (!samePos || emergencyCounter < 100)
            {
                if (emptyTile.x == ogPos.x && emptyTile.y == ogPos.y)
                {
                    samePos = true;
                    break;
                }
                //Debug.Log(emergencyCounter + " action is moving (WT)" + workingTile + " to (ET)" + emptyTile);
                //Display movement
                LeanTween.move(activePieces[workingTile.x, workingTile.y].gameObject, new Vector3(emptyTile.x, emptyTile.y, 0), 0.25f).setOnComplete(()=> {
                    CheckForEndGame();
                });
                //update gridValues
                SlidingPiece emptyPiece = activePieces[emptyTile.x, emptyTile.y];
                activePieces[emptyTile.x, emptyTile.y] = activePieces[workingTile.x, workingTile.y];
                activePieces[workingTile.x, workingTile.y] = emptyPiece;
                //calculate next moving piece
                emptyTile = workingTile;
                workingTile -= direction;
                //Debug.Log("(ET)" + emptyTile + " | (OG)" + ogPos);
                emergencyCounter++;
            }
        }
    }

    private void CheckForEndGame()
    {
        bool canEndGame = true;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if(activePieces[x,y].TGTPos.x != x && activePieces[x, y].TGTPos.y != y)
                {
                    Debug.Log("has not completed game!");
                    canEndGame = false;
                    return;
                }
            }
        }

        if(canEndGame)
        {
            Debug.Log("GameCompleted");
            SceneManager.LoadScene(0);
        }
    }

    public Vector2Int GetPieceAtPointerPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (Physics2D.OverlapCircle(mousePos, 0.05f, piecesLayer) == null)
        {
            Debug.Log("No collider clicked");
            return -Vector2Int.one;
        }

        return new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
    }

    private void SetupGrid()
    {
        //Spawn pieces
        List<SlidingPiece> spawnedPieces = new List<SlidingPiece>();
        int counter = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject go = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity);
                go.transform.parent = transform;
                go.GetComponent<SpriteRenderer>().sprite = pieces[counter];
                SlidingPiece sp = go.GetComponent<SlidingPiece>();
                sp.TGTPos = new Vector2Int(x, y);
                spawnedPieces.Add(sp);
                counter++;
            }
        }
        
        //Mix pieces
        spawnedPieces.Shuffle();

        //Place pieces
        activePieces = new SlidingPiece[gridSize.x, gridSize.y];
        counter = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                spawnedPieces[counter].transform.position = new Vector3(x, y, 0);
                activePieces[x, y] = spawnedPieces[counter];
                counter++;
            }
        }

        emptyTile = centerPieceCoord;
        hiddenPiece = activePieces[centerPieceCoord.x, centerPieceCoord.y];
        hiddenPiece.gameObject.SetActive(false);
    }
}
