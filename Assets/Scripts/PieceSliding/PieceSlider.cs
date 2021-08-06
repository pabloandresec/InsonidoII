using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PieceSlider : Puzzle
{
#pragma warning disable 0649

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2Int centerPieceCoord;
    [SerializeField] private Sprite[] pieces;
    [SerializeField] private Sprite full;
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private float gridBorder = 1f;
    [SerializeField] private float lerpTime = 0.25f;
    [SerializeField] private MixMode mixMode = MixMode.MixA;
    [SerializeField] private int mixAmount = 100;
    [SerializeField] private LayerMask piecesLayer;
    [SerializeField] private SpriteRenderer board;
    [SerializeField] private Vector3 cameraOffset;

    private SlidingPiece[,] activePieces;
    private Vector2Int emptyTile = Vector2Int.zero;
    private SlidingPiece hiddenPiece;
    private int idx = 0;
    private bool blockInput = false;
    private bool sliding = false;


#pragma warning restore 0649


    public override void StartPuzzle()
    {
        base.StartPuzzle();
        blockInput = true;
        Utils.SetCameraInMiddleOfGrid(cameraOffset, Camera.main, gridSize, gridBorder);
        SetupGrid();
        ScaleBorder();

        ResizeSpriteToScreen();
    }

    private void ScaleBorder()
    {
        Vector3 boardCenter = new Vector3(((float)gridSize.x / 2) - 0.5f, ((float)gridSize.y / 2) - 0.5f, 0);
        Vector2 boardSize = new Vector2(gridSize.x + 0.5f, gridSize.y + 0.5f);
        board.transform.position = boardCenter;
        board.size = boardSize;
        board.transform.GetChild(0).position = boardCenter;
        board.transform.GetChild(0).localScale = board.transform.localScale;
        board.transform.GetChild(0).GetComponent<SpriteRenderer>().size = boardSize;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (blockInput)
            {
                Debug.Log("Locked Input!!");
                return;
            }
            Vector2Int clickPosition = GetPieceAtPointerPos();
            if (clickPosition != -Vector2Int.one)
            {
                CheckForSlide(clickPosition, lerpTime, () => { CheckForEndGame(); });
            }
        }
        /*
        if (Input.GetKeyDown(KeyCode.D))
        {
            activePieces = FixPuzzle();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckForEndGame();
        }
        */
    }



    /*
    private void CheckForSlide(Vector2Int coords)
    {
        if(sliding)
        {
            return;
        }
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
            sliding = true;
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
                LeanTween.move(activePieces[workingTile.x, workingTile.y].gameObject, new Vector3(emptyTile.x, emptyTile.y, 0), lerpTime).setOnComplete(()=> {
                    CheckForEndGame();
                    sliding = false;
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
    */
    private void CheckForSlide(Vector2Int coords, float leantime, Action onEnd)
    {
        if (sliding)
        {
            //Debug.Log("Sliding Already!");
            return;
        }
        bool canSlide = false;
        if (coords.y == emptyTile.y)
        {
            canSlide = true;
        }
        if (coords.x == emptyTile.x)
        {
            canSlide = true;
        }
        if (canSlide)
        {
            sliding = true;
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
                LeanTween.move(activePieces[workingTile.x, workingTile.y].gameObject, new Vector3(emptyTile.x, emptyTile.y, 0), leantime).setOnComplete(() => {
                    sliding = false;
                    onEnd?.Invoke();
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
        else
        {
            Debug.Log("Cannot slide from " + coords);
        }
    }

    private void CheckForEndGame()
    {
        bool canEndGame = true;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                SlidingPiece sp = activePieces[x, y];
                if (sp.TGTPos != new Vector2Int(x,y))
                {
                    Debug.Log("has not completed game!");
                    canEndGame = false;
                    return;
                }
            }
        }

        if(canEndGame)
        {
            EndGame();
            //SceneManager.LoadScene(0);
        }
    }

    private void EndGame()
    {
        activePieces[emptyTile.x, emptyTile.y].gameObject.SetActive(true);
        transform.SetParent(board.transform);
        LeanTween.scale(board.gameObject, transform.localScale * 1.1f, 0.2f).setLoopPingPong(2);
        Debug.Log("GameCompleted");
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

        switch(mixMode)
        {
            case MixMode.MixA:
                MixA(spawnedPieces);
                break;
            case MixMode.MixB:
                MixB(spawnedPieces);
                break;
        }
    }

    private void MixB(List<SlidingPiece> spawnedPieces)
    {
        //Init pieces
        activePieces = new SlidingPiece[gridSize.x, gridSize.y];
        int counter = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                spawnedPieces[counter].transform.position = new Vector3(x, y, 0);
                activePieces[x, y] = spawnedPieces[counter];
                counter++;
            }
        }

        PlaceEmptyCell();

        StartCoroutine(MixingRoutine(mixAmount));
    }

    private IEnumerator MixingRoutine(int amount)
    {
        //Debug.Log("Mixing "+ amount);
        idx = 0;
        while (idx < amount)
        {
            Vector2Int gen = GetValidCoord();
            CheckForSlide(gen, 0.05f, () => {
                idx++;
                //Debug.Log("trying to move to " + gen);
            });
            yield return null;
        }
        blockInput = false;
    }

    private Vector2Int GetValidCoord()
    {
        Vector2Int selectedPos = Vector2Int.zero;
        List<int> validIndexes = new List<int>();
        bool isX = Random.value < 0.5f ? true : false;
        if (isX)
        {
            for (int indx = 0; indx < gridSize.x; indx++)
            {
                if (indx != emptyTile.x)
                {
                    validIndexes.Add(indx);
                }
            }
            selectedPos = new Vector2Int(validIndexes[Random.Range(0, validIndexes.Count)], emptyTile.y);
        }
        else
        {
            for (int indx = 0; indx < gridSize.y; indx++)
            {
                if (indx != emptyTile.y)
                {
                    validIndexes.Add(indx);
                }
            }
            selectedPos = new Vector2Int(emptyTile.x, validIndexes[Random.Range(0, validIndexes.Count)]);
        }
        return selectedPos;
    }

    private void PlaceEmptyCell()
    {
        emptyTile = centerPieceCoord;
        hiddenPiece = activePieces[centerPieceCoord.x, centerPieceCoord.y];
        hiddenPiece.gameObject.SetActive(false);
    }

    private void MixA(List<SlidingPiece> spawnedPieces)
    {
        //Mix pieces
        spawnedPieces.Shuffle();

        //Init pieces
        activePieces = new SlidingPiece[gridSize.x, gridSize.y];
        int counter = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                spawnedPieces[counter].transform.position = new Vector3(x, y, 0);
                activePieces[x, y] = spawnedPieces[counter];
                counter++;
            }
        }
        PlaceEmptyCell();
    }
    private SlidingPiece[,] FixPuzzle()
    {
        SlidingPiece[,] fixedTable = new SlidingPiece[gridSize.x, gridSize.y];


        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2Int tgtPos = activePieces[x, y].TGTPos;
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
}

public enum MixMode
{
    MixA,
    MixB
}
