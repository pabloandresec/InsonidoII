﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PathFinderPuzzle : Puzzle
{
#pragma warning disable 0649

    //Vars
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private GameObject[] pieces;
    [SerializeField] private GameObject shapePrefab;
    [SerializeField] private GameObject probePrefab;
    [SerializeField] private GameObject colTester;
    [SerializeField] private CGrid gridPather;
    [SerializeField] private Transform character;
    private Vector2Int startPos;
    private Vector2Int endPos;//
    [SerializeField] private Transform discardParent;
    [SerializeField] private Transform piecesParent;
    [SerializeField] private Transform probesParent;
    [SerializeField] private int[] rotations = new int[] { 0, 90, 180, 270 };

    [Header("Camera")]
    [SerializeField] private float gridBorder;
    [SerializeField] private SpriteRenderer board;
    [SerializeField] private float boardBorderSize;

    [Header("Path")]
    [SerializeField] private List<Vector2> path;

    private Vector2[] firstPath;
    private Vector2[] secondPath;
    private bool firstPathFound = false;
    private bool secondPathFound = false;
    Vector2Int middleSpot;
    private Vector2 startNode, endNode;
    //Props
    public Vector2Int GridSize { get => gridSize; }
#pragma warning restore 0649

    public override void StartPuzzle()
    {
        base.StartPuzzle();
        GeneratePath();

        Utils.SetCameraInMiddleOfGrid(new Vector3(1,1,0), Camera.main, gridSize, gridBorder);
        board.transform.position = new Vector3(((float)gridSize.x / 2) - 0.5f, ((float)gridSize.y / 2) - 0.5f, 0);
        board.size = new Vector2(gridSize.x + boardBorderSize, gridSize.y + boardBorderSize);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            Collider2D col = Physics2D.OverlapCircle(worldPos, 0.2f);
            if (col != null)
            {
                col.GetComponent<PathPiece>().RotatePiece(() => {
                    CheckForPathCompletition();
                });
            }
        }
    }

    private void CheckForPathCompletition()
    {
        for (int i = 0; i < piecesParent.childCount; i++)
        {
            PathPiece pp = piecesParent.GetChild(i).GetComponent<PathPiece>();
            pp.ToggleColliderForPathfinding();
        }

        gridPather.Scan();
        firstPathFound = false;
        Vector2[] testPath = Pathfinding.RequestPath(startPos, endPos, "MAIN", () =>
        {
            firstPathFound = true;
        });
        StartCoroutine(CheckPath(testPath));
    }

    private IEnumerator CheckPath(Vector2[] testPath)
    {
        while(!firstPathFound)
        {
            yield return null;
        }
        if(testPath.Length > 1)
        {
            if (testPath[testPath.Length - 1] == endNode)
            {
                Debug.Log("GAME COMPLETED!");
                SceneManager.LoadScene(0);
            }
            else
            {
                Debug.Log("End node unreachable!");
                for (int i = 0; i < piecesParent.childCount; i++)
                {
                    PathPiece pp = piecesParent.GetChild(i).GetComponent<PathPiece>();
                    pp.ToggleColliderForRotation();
                }
            }
        }
        else
        {
            Debug.Log("End node unreachable!");
            for (int i = 0; i < piecesParent.childCount; i++)
            {
                PathPiece pp = piecesParent.GetChild(i).GetComponent<PathPiece>();
                pp.ToggleColliderForRotation();
            }
        }
    }

    private void GeneratePath()
    {
        transform.position = new Vector3(((float)gridSize.x / 2) - 0.5f, ((float)gridSize.y / 2) - 0.5f, 0);

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject inst = Instantiate(shapePrefab, new Vector3(x, y, 0), Quaternion.identity);
                inst.transform.SetParent(discardParent);
            }
        }

        //PlaceHindrance();

        gridPather.Init((Vector2)gridSize);
        gridPather.Scan(null);

        startPos = new Vector2Int(-1, gridSize.y - 1);
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.transform.position = new Vector3(startPos.x, startPos.y, 0);
        s.transform.localScale = new Vector3(1, 0.5f, 1);
        endPos = new Vector2Int(gridSize.x , 0);
        GameObject e = GameObject.CreatePrimitive(PrimitiveType.Cube);
        e.transform.position = new Vector3(endPos.x, endPos.y, 0);
        e.transform.localScale = new Vector3(1, 0.5f, 1);

        middleSpot = GetRandomGridPos(new Vector2Int[] { new Vector2Int(0, gridSize.y - 1), new Vector2Int(gridSize.x - 1, 0) });

        firstPath = Pathfinding.RequestPath(startPos, middleSpot, "MAIN", () =>
        {
            firstPathFound = true;
        });
        secondPath = Pathfinding.RequestPath(middleSpot, endPos, "MAIN", () =>
        {
            secondPathFound = true;
        });
        StartCoroutine(EndGeneration());
    }

    private IEnumerator EndGeneration()
    {
        while(!firstPathFound && !secondPathFound)
        {
            yield return null;
        }
        path = new List<Vector2>();
        path = firstPath.ToList();
        path.AddRange(secondPath);


        //Debug.LogError("Starting "+ path.Length +" probes");
        for (int i = 0; i < path.Count; i++)
        {
            //Debug.Log("Spawning probe " + i);
            GameObject prob = Instantiate(probePrefab, path[i], Quaternion.identity);
            prob.transform.name = "Probe_" + i;
            prob.transform.SetParent(probesParent);
        }
        startNode = path[0];
        endNode = path[path.Count - 1];
        //discardParent.gameObject.SetActive(false);
        Destroy(discardParent.gameObject);
        SpawnPieces();

        probesParent.gameObject.SetActive(false);
        for (int i = 0; i < piecesParent.childCount; i++)
        {
            piecesParent.GetChild(i).gameObject.SetActive(true);
            piecesParent.GetChild(i).GetComponent<PathPiece>().ToggleColliderForRotation();
        }
    }

    private Vector2Int GetRandomGridPos(Vector2Int[] excludingGridCoords)
    {
        Vector2Int val = Vector2Int.zero;
        bool done = false;
        while (!done)
        {
            val = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            bool isConflicted = false;
            foreach (Vector2Int v in excludingGridCoords)
            {
                if (val == v)
                {
                    isConflicted = true;
                }
            }
            if (!isConflicted)
            {
                done = true;
                break;
            }
        }
        return val;
    }

    private void SpawnPieces()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 cellPos = new Vector3(x, y, 0);
                Collider2D[] col = Physics2D.OverlapCircleAll(cellPos, 0.5f);

                if (col.Length > 4)
                {
                    SpawnCorrectPiece(cellPos);
                    
                }
                else
                {
                    SpawnRandomPiece(cellPos);
                }
            }
        }
    }

    private void SpawnRandomPiece(Vector3 cellPos)
    {
        int randomPieceIndex = Random.Range(0, pieces.Length);
        Vector3 rot = new Vector3(0, 0, rotations[Random.Range(0, rotations.Length)]);
        GameObject randomPiece = Instantiate(pieces[randomPieceIndex], cellPos, Quaternion.Euler(rot));
        randomPiece.transform.name = "RandomPiece" + cellPos;
        randomPiece.SetActive(false);
        randomPiece.transform.SetParent(piecesParent);
    }

    private void SpawnCorrectPiece(Vector3 cellPos)
    {
        bool correctPiece = false;
        PathPiece testingPiece = null;

        for (int i = 0; i < rotations.Length; i++)
        {
            for (int j = 0; j < pieces.Length; j++)
            {
                testingPiece = Instantiate(pieces[j], cellPos, Quaternion.identity).GetComponent<PathPiece>();
                testingPiece.transform.eulerAngles = new Vector3(0, 0, rotations[i]);
                correctPiece = testingPiece.TestPoints();
                if(correctPiece)
                {
                    testingPiece.transform.name = "CorrectPiece" + cellPos;
                    testingPiece.transform.SetParent(piecesParent);
                    testingPiece.gameObject.SetActive(false);
                    testingPiece.transform.eulerAngles = new Vector3(0, 0, rotations[Random.Range(0,rotations.Length)]);
                    return;
                }
                //Debug.Log(pieces[j] + " Does not correspond!");
                Destroy(testingPiece.gameObject);
                testingPiece = null;
            }
        }
    }

    private void PlaceHindrance()
    {
        throw new NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        if(path.Count > 0)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
        Gizmos.DrawSphere((Vector2)startPos, 0.25f);
        Gizmos.DrawSphere((Vector2)endPos, 0.25f);
    }
}
