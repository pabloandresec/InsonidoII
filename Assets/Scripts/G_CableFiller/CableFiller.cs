using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class CableFiller : Puzzle
{
#pragma warning disable 0649


    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private SnappingPoint[,] grid;
    [SerializeField] private GameObject snappingPointPrefab;
    [Header("--------------SEGMENTOS--------------")]
    [SerializeField] private CableSegment[] segments;
    [Header("-------------------------------------")]
    [SerializeField] private GameObject cablePointPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform originPointsParent;
    [SerializeField] private Transform linesParent;
    [SerializeField] private float gridBorder;
    [SerializeField] private SpriteRenderer board;
    [Header("Barra")]
    [SerializeField] private Transform bar;
    [SerializeField] private Color barEmpty;
    [SerializeField] private Color barFull;
    [Header("Sonidos")]
    [SerializeField] private AudioClip onSnap;
    [SerializeField] private AudioClip onGameCompleted;
    [SerializeField] private AudioClip onCableSnap;
    [SerializeField] private FloatRange pitchRange;

    private AudioController ac;
    private SpriteRenderer barRend;
    private float maxBarScale = 5f;
    private int currentSegment = -1;
    private Vector2Int lineStart = Vector2Int.zero;
    private bool drawing = false;
    //sound
    private float currentPitch;
    private int snapPointsAmount;

#pragma warning restore 0649


    public override void StartPuzzle()
    {
        base.StartPuzzle();
        GenerateGrid();
        Utils.SetCameraInMiddleOfGrid(new Vector3(1, 1, 0), Camera.main, gridSize, gridBorder);
        board.transform.position = new Vector3(((float)gridSize.x / 2) - 0.5f, ((float)gridSize.y / 2) - 0.5f, 0);
        board.size = new Vector2(gridSize.x + 0.5f, gridSize.y + 0.5f);
        ResizeSpriteToScreen(Camera.main);
        bar.parent.GetComponent<SpriteRenderer>().size = new Vector2(gridSize.x + 0.5f, bar.parent.GetComponent<SpriteRenderer>().size.y);
        bar.localScale = new Vector3(gridSize.x, bar.localScale.y);
        maxBarScale = bar.localScale.x;
        snapPointsAmount = gridSize.x * gridSize.y;
        barRend = bar.GetComponent<SpriteRenderer>();
        ac = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
        LerpBar(0f);
    }

    private void Update()
    {
        if(paused)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            StartDrawingInstructions();
        }
        if (drawing)
        {
            Vector2Int pointerPos = GetPointerRoundedWorldPositions();

            if (Physics2D.OverlapCircle(pointerPos, 0.25f))
            {
                if (segments[currentSegment].LinePoints.Contains(new Vector3(pointerPos.x, pointerPos.y, 0))) // si tocaste el mismo cable que estas haciendo
                {
                    ReformatSegment(currentSegment, pointerPos);
                    ac.PlaySFX(onCableSnap, 1);
                }

                if (IsOverlappingWithDifferentColorOrigin(pointerPos, currentSegment, grid[pointerPos.x, pointerPos.y].SegIndex)) // si se solapa la linea con un origen de distinto color
                {
                    CancelDrawing();
                    ac.PlaySFX(onCableSnap, 1);
                    return;
                }
                if (IsOverlappingWithDifferentColorLine(pointerPos, currentSegment)) // si se solapa la linea con otra de diferente color
                {
                    //Debug.Log("Overlapping with different color");
                    if (grid[pointerPos.x, pointerPos.y].SegIndex >= 0)
                    {
                        Debug.Log("Crossed anothe line");
                        ReformatSegment(grid[pointerPos.x, pointerPos.y].SegIndex, pointerPos);
                        ac.PlaySFX(onCableSnap, 1);
                    }
                }
                int overlappingIndex = -1;
                currentPitch = Utils.ConvertRangedValueToAnotherRange(segments[currentSegment].Line.positionCount, new FloatRange(0, snapPointsAmount), pitchRange);
                if (IsOverlappingWithAnyOrigin(pointerPos, out overlappingIndex)) //Si toca cualquiera de los origenes
                {
                    if (overlappingIndex == currentSegment)
                    {
                        DrawLine(pointerPos);
                        ac.PlaySFX(onSnap, currentPitch);
                        CancelDrawing();
                        return;
                    }
                }
                ac.PlaySFX(onSnap, currentPitch);
                DrawLine(pointerPos);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            currentPitch = pitchRange.min;
            CancelDrawing();
            CheckForCompletition();
        }
    }

    private void StartDrawingInstructions()
    {
        Vector2Int pointerPos = GetPointerRoundedWorldPositions();
        //currentSegment = CheckForPoints(pointerPos, out lineStart);
        currentSegment = CheckForPoints(pointerPos);
        if (currentSegment != -1)
        {
            drawing = true;
            EnableNeighboursOnly(pointerPos);
            if (segments[currentSegment].HasActiveLine())
            {
                if (segments[currentSegment].GetLastPosition() != pointerPos || IsOverlappingWithAnyOrigin(pointerPos))
                {
                    DeleteLine(segments[currentSegment]);
                }
            }
            segments[currentSegment].LinePoints.Add(new Vector3(pointerPos.x, pointerPos.y, 0));
            segments[currentSegment].UpdatePoints();
            segments[currentSegment].Line.enabled = true;
            //Debug.Log(currentSegment + " index segment selected");
        }
    }

    private void DrawLine(Vector2Int pointerPos)
    {
        SetBarAmount();
        EnableNeighboursOnly(pointerPos);
        grid[pointerPos.x, pointerPos.y].SetSegmentIndex(currentSegment);
        segments[currentSegment].LinePoints.Add(new Vector3(pointerPos.x, pointerPos.y, 0));
        segments[currentSegment].UpdatePoints();
    }

    private void SetBarAmount()
    {
        int validPoints = 0;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if (grid[x, y].SegIndex != -1)
                {
                    validPoints++;
                }
            }
        }


        float lerpValue = Mathf.InverseLerp(0, gridSize.x * gridSize.y, validPoints);
        //Debug.Log("valid p = " + validPoints + ", Lerp val" + lerpValue);
        LerpBar(lerpValue);
    }

    private void LerpBar(float lerpValue)
    {
        bar.localScale = new Vector3(Mathf.Lerp(0, maxBarScale, lerpValue), bar.localScale.y, bar.localScale.z);
        barRend.color = Color.Lerp(barEmpty, barFull, lerpValue);
    }

    private void CheckForCompletition()
    {
        bool complete = true;

        for (int i = 0; i < segments.Length; i++)
        {
            Vector3 startPos = new Vector3(segments[i].Start.x, segments[i].Start.y, 0);
            Vector3 endPos = new Vector3(segments[i].End.x, segments[i].End.y, 0);
            if (!segments[i].LinePoints.Contains(endPos) || !segments[i].LinePoints.Contains(startPos))
            {
                complete = false;
            }
        }

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if(grid[x,y].SegIndex < 0)
                {
                    complete = false;
                }
            }
        }

        if(complete)
        {
            Debug.Log("Level Complete");
            ac.PlaySFX(onGameCompleted);
            SceneManager.LoadScene(0);
        }
    }

    private void DeleteLine(CableSegment cableSegment)
    {
        for (int i = 0; i < cableSegment.Line.positionCount; i++)
        {
            Vector3 lineWorldPos = cableSegment.Line.GetPosition(i);
            Vector2Int gPos = new Vector2Int((int)lineWorldPos.x, (int)lineWorldPos.y);
            if(!IsOverlappingWithAnyOrigin(gPos))
            {
                grid[gPos.x, gPos.y].SetSegmentIndex(-1);
            }
            cableSegment.LinePoints.Remove(lineWorldPos);
        }
        cableSegment.Line.positionCount = 0;
        cableSegment.UpdatePoints();
    }

    private void ReformatSegment(int idx, Vector2Int pos)
    {
        for (int i = segments[idx].Line.positionCount - 1; i >= 0; i--)
        {
            Vector3 tPos = segments[idx].Line.GetPosition(i);

            if(!IsOverlappingWithAnyOrigin(new Vector2Int((int)tPos.x, (int)tPos.y)))
            {
                grid[(int)tPos.x, (int)tPos.y].SetSegmentIndex(-1);
            }
            segments[idx].LinePoints.Remove(tPos);
            if (segments[idx].Line.GetPosition(i) == new Vector3(pos.x, pos.y, 0))
            {
                break;
            }
        }
        segments[idx].UpdatePoints();
    }

    private bool IsOverlappingWithAnyOrigin(Vector2Int pos, out int segmentIndex)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i].Start == pos || segments[i].End == pos)
            {
                segmentIndex = i;
                return true;
            }
        }
        segmentIndex = -1;
        return false;
    }

    private bool IsOverlappingWithAnyOrigin(Vector2Int pos)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i].Start == pos || segments[i].End == pos)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsOverlappingWithDifferentColorLine(Vector2Int pointerPos, int currentSegment)
    {
        return currentSegment != grid[pointerPos.x, pointerPos.y].SegIndex;
    }

    private bool IsOverlappingWithDifferentColorOrigin(Vector2Int pos, int currentLineIndex, int posIndex)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i].Start == pos || segments[i].End == pos)
            {
                if (currentLineIndex != posIndex)
                {
                    //Debug.Log("Overlapping with different color");
                    return true;
                }
            }
        }
        return false;
    }

    private void CancelDrawing()
    {
        drawing = false;
        SetAllColliders(true);
        //Debug.Log(currentSegment + " index segment deselected");
        currentSegment = -1;
    }

    private void EnableNeighboursOnly(Vector2Int pos)
    {
        SetAllColliders(false);
        for (int i = 0; i < Utils.cardDirections.Length; i++)
        {
            Vector2Int probedPos = pos + Utils.cardDirections[i];
            if(Utils.IsInBounds(probedPos,gridSize))
            {
                grid[probedPos.x, probedPos.y].SetCollider(true);
            }
        }
    }

    private void SetAllColliders(bool state)
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                grid[x, y].SetCollider(state);
            }
        }
    }

    private Vector2Int GetPointerRoundedWorldPositions()
    {
        Vector3 worldPosPointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int pointerPos = new Vector2Int(Mathf.RoundToInt(worldPosPointer.x), Mathf.RoundToInt(worldPosPointer.y));
        return pointerPos;
    }

    private int CheckForPoints(Vector2Int pointerPoint)
    {
        if(!Utils.IsInBounds(pointerPoint, gridSize))
        {
            return -1;
        }
        return grid[pointerPoint.x, pointerPoint.y].SegIndex;
    }

    private void GenerateGrid()
    {
        ClearParents();

        grid = new SnappingPoint[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject snappingPoint = Instantiate(snappingPointPrefab, new Vector3(x, y, 0), Quaternion.identity);
                snappingPoint.transform.name = "SnappingPoint_" + x + "," + y;
                snappingPoint.transform.SetParent(transform);
                grid[x, y] = snappingPoint.GetComponent<SnappingPoint>();
                grid[x, y].Init(-1);
            }
        }

        foreach (Transform child in originPointsParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        for (int i = 0; i < segments.Length; i++)
        {
            GameObject startPoint = Instantiate(cablePointPrefab, new Vector3(segments[i].Start.x, segments[i].Start.y, 0), Quaternion.identity);
            grid[segments[i].Start.x, segments[i].Start.y].SetSegmentIndex(i);
            startPoint.GetComponent<SpriteRenderer>().color = segments[i].Color;
            startPoint.transform.SetParent(originPointsParent);
            startPoint.transform.name = i + "_SegmentStart";
            GameObject endPoint = Instantiate(cablePointPrefab, new Vector3(segments[i].End.x, segments[i].End.y, 0), Quaternion.identity);
            grid[segments[i].End.x, segments[i].End.y].SetSegmentIndex(i);
            endPoint.GetComponent<SpriteRenderer>().color = segments[i].Color;
            endPoint.transform.SetParent(originPointsParent);
            endPoint.transform.name = i + "_SegmentEnd";
        }

        for (int i = 0; i < segments.Length; i++)
        {
            LineRenderer lr = Instantiate(linePrefab, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
            lr.transform.name = i + "_LineRenderer";
            lr.transform.SetParent(linesParent);
            segments[i].Init(lr);
        }
    }

    private void OnDisable()
    {
        ClearParents();
    }

    private void ClearParents()
    {
        if(grid != null)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    DestroyImmediate(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        if (linesParent != null && linesParent.childCount > 0)
        {
            
            for (int i = 0; i < segments.Length; i++)
            {
                if(segments[i].Line != null)
                {
                    DestroyImmediate(segments[i].Line.gameObject);
                    segments[i].Line = null;
                }
            }
        }

        if(originPointsParent != null)
        {
            foreach (Transform child in originPointsParent.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }


    public void ReGenerateGrid()
    {
        OutOfIndexCheck();
        GenerateGrid();
    }

    private void OutOfIndexCheck()
    {
        if (gridSize.x < 2)
        {
            gridSize.x = 2;
        }
        if (gridSize.y < 2)
        {
            gridSize.y = 2;
        }
        foreach (CableSegment cs in segments)
        {
            if (cs.Start.x < 0)
            {
                cs.Start = new Vector2Int(0, cs.Start.y);
            }
            if (cs.Start.y < 0)
            {
                cs.Start = new Vector2Int(cs.Start.x, 0);
            }
            if (cs.End.x < 0)
            {
                cs.End = new Vector2Int(0, cs.End.y);
            }
            if (cs.End.y < 0)
            {
                cs.End = new Vector2Int(cs.End.x, 0);
            }

            if (cs.Start.x > gridSize.x)
            {
                cs.Start = new Vector2Int(gridSize.x - 1, cs.Start.y);
            }
            if (cs.Start.y > gridSize.y)
            {
                cs.Start = new Vector2Int(cs.Start.x, gridSize.y - 1);
            }
            if (cs.End.x > gridSize.x)
            {
                cs.End = new Vector2Int(gridSize.x - 1, cs.End.y);
            }
            if (cs.End.y > gridSize.y)
            {
                cs.End = new Vector2Int(cs.End.x, gridSize.y - 1);
            }
        }
    }

    public override void PauseGame()
    {
        Debug.Log("PAUSING GAME");
        GameObject.FindGameObjectWithTag("UI").GetComponent<MenuController>().SwapMenu(1);
        paused = true;
    }

    public override void ResumeGame()
    {
        Debug.Log("RESUMING GAME");
        GameObject.FindGameObjectWithTag("UI").GetComponent<MenuController>().SwapMenu(0);
        paused = false;
    }

    public override void RestartGame()
    {
        Debug.Log("RESETTING GAME");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
