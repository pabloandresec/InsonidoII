using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CGrid : MonoBehaviour {

#pragma warning disable 0649

    [SerializeField] private bool diagonals = true;
    [SerializeField] private bool debug;
    [SerializeField] private LayerMask unwalkableMask;
    [SerializeField] private float nodeRadius;

    private Vector2 gridWorldSize;
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    public bool Diagonals { get => diagonals; }

#pragma warning restore 0649


    public void Init(Vector2 _gridWorldSize)
    {
        GetComponent<Pathfinding>().Init();
        nodeDiameter = nodeRadius * 2;
        gridWorldSize = _gridWorldSize;
        gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / nodeDiameter);
    }

    public void Scan(Action onScanEnd) {
		grid = new Node[gridSizeX,gridSizeY];
		Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x/2 - Vector2.up * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = Physics2D.OverlapCircle(worldPoint, 0.01f);

                grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
        Debug.LogWarning("Map Scan finished");
        onScanEnd?.Invoke();
    }

    public void Scan()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = Physics2D.OverlapCircle(worldPoint, 0.01f);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
        Debug.LogWarning("Map Scan finished");
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public List<Node> GetNeighbours(Node node, int depth = 1) {
		List<Node> neighbours = new List<Node>();

        if(diagonals)
        {
            for (int x = -depth; x <= depth; x++)
            {
                for (int y = -depth; y <= depth; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
        else
        {
            int checkX, checkY;
            //Check Top
            checkX = node.gridX;
            checkY = node.gridY + 1;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(grid[checkX, checkY]);
            }
            //Check Bottom
            checkX = node.gridX;
            checkY = node.gridY - 1;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(grid[checkX, checkY]);
            }
            //Check left
            checkX = node.gridX - 1;
            checkY = node.gridY;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(grid[checkX, checkY]);
            }
            //Check right
            checkX = node.gridX + 1;
            checkY = node.gridY;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(grid[checkX, checkY]);
            }
        }
		

		return neighbours;
	}
	
    public Node NodeFromWorldPoint(Vector3 _worldPosition)
    {
        float posX = ((_worldPosition.x - transform.position.x) + gridWorldSize.x * 0.5f) / nodeDiameter;
        float posY = ((_worldPosition.y - transform.position.y) + gridWorldSize.y * 0.5f) / nodeDiameter;

        posX = Mathf.Clamp(posX, 0, gridSizeX - 1);
        posY = Mathf.Clamp(posY, 0, gridSizeY - 1);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);

        return grid[x, y];
    }


    public Node ClosestWalkableNode(Node node) {
		int maxRadius = Mathf.Max (gridSizeX, gridSizeY) / 2;
		for (int i = 1; i < maxRadius; i++) {
            //Node n = FindWalkableInRadius (node.gridX, node.gridY, i);
            Node n = FindWalkableInRadius(new Vector2Int(node.gridX, node.gridY), i);
            if (n != null) {
				return n;

			}
		}
		return null;
	}
	Node FindWalkableInRadius(int centreX, int centreY, int radius) {

		for (int i = -radius; i <= radius; i ++) {
			int verticalSearchX = i + centreX;
			int horizontalSearchY = i + centreY;

			// top
			if (InBounds(verticalSearchX, centreY + radius)) {
				if (grid[verticalSearchX, centreY + radius].walkable) {
					return grid [verticalSearchX, centreY + radius];
				}
			}

			// bottom
			if (InBounds(verticalSearchX, centreY - radius)) {
				if (grid[verticalSearchX, centreY - radius].walkable) {
					return grid [verticalSearchX, centreY - radius];
				}
			}
			// right
			if (InBounds(centreY + radius, horizontalSearchY)) {
				if (grid[centreX + radius, horizontalSearchY].walkable) {
					return grid [centreX + radius, horizontalSearchY];
				}
			}

			// left
			if (InBounds(centreY - radius, horizontalSearchY)) {
				if (grid[centreX - radius, horizontalSearchY].walkable) {
					return grid [centreX - radius, horizontalSearchY];
				}
			}

		}

		return null;

	}

    Node FindWalkableInRadius(Vector2Int pos, int radius)
    {

        for (int i = -radius; i <= radius; i++)
        {
            int verticalSearchX = i + pos.x;
            int horizontalSearchY = i + pos.y;

            // top
            if (InBounds(verticalSearchX, pos.y + radius))
            {
                if (grid[verticalSearchX, pos.y + radius].walkable)
                {
                    return grid[verticalSearchX, pos.y + radius];
                }
            }

            // bottom
            if (InBounds(verticalSearchX, pos.y - radius))
            {
                if (grid[verticalSearchX, pos.y - radius].walkable)
                {
                    return grid[verticalSearchX, pos.y - radius];
                }
            }
            // right
            if (InBounds(pos.x + radius, horizontalSearchY))
            {
                if (grid[pos.x + radius, horizontalSearchY].walkable)
                {
                    return grid[pos.x + radius, horizontalSearchY];
                }
            }

            // left
            if (InBounds(pos.x - radius, horizontalSearchY))
            {
                if (grid[pos.x - radius, horizontalSearchY].walkable)
                {
                    return grid[pos.x - radius, horizontalSearchY];
                }
            }

        }

        return null;

    }

    private bool InBounds(int x, int y) {
		return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
	}

    private bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSizeX && pos.y >= 0 && pos.y < gridSizeY;
    }

    void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector2(gridWorldSize.x,gridWorldSize.y));
		if (grid != null && debug) {
			foreach (Node n in grid) {
				Gizmos.color = Color.red;
				if (n.walkable)
					Gizmos.color = Color.white;

				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}

}