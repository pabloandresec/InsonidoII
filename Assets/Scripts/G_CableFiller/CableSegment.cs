using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CableSegment
{
#pragma warning disable 0649

    [SerializeField] private Vector2Int start;
    [SerializeField] private Vector2Int end;
    [SerializeField] private Color color;
    private HashSet<Vector3> linePoints;
    private LineRenderer line;

    public Vector2Int Start { get => start; set => start = value; }
    public Vector2Int End { get => end; set => end = value; }
    public Color Color { get => color; set => color = value; }
    public HashSet<Vector3> LinePoints { get => linePoints; set => linePoints = value; }
    public LineRenderer Line { get => line; set => line = value; }

#pragma warning restore 0649


    public void Init(LineRenderer line)
    {
        Line = line;
        line.startColor = color;
        line.endColor = color;
        linePoints = new HashSet<Vector3>();
    }

    public void UpdatePoints()
    {
        line.positionCount = linePoints.Count;
        line.SetPositions(linePoints.ToArray());
    }

    public bool HasActiveLine()
    {
        return line.positionCount > 0;
    }

    public Vector2Int GetLastPosition()
    {
        Vector3 pos = line.GetPosition(line.positionCount - 1);
        return new Vector2Int((int)pos.x, (int)pos.y);
    }
}
