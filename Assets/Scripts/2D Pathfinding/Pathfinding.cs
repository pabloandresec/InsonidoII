using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;

public class Pathfinding : MonoBehaviour {

	private CGrid grid;
	public static Pathfinding instance;
	
	public void Init() {
		grid = GetComponent<CGrid>();
		instance = this;
	}

    public CGrid GetGrid()
    {
        return grid;
    }

	public static Vector2[] RequestPath(Vector2 from, Vector2 to, string requester) {
        Debug.LogError(requester + " asked for path from " + from + " to " + to);
		return instance.FindPath (from, to, null);
	}

    public static Vector2[] RequestPath(Vector2 from, Vector2 to, string requester, Action OnPathFound)
    {
        //Debug.LogError(requester + " asked for path from " + from + " to " + to);
        return instance.FindPath(from, to, OnPathFound);
    }

    Vector2[] FindPath(Vector2 from, Vector2 to, Action OnPathFound) {
		
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(from);
		Node targetNode = grid.NodeFromWorldPoint(to);

        if(startNode == targetNode)
        {
            Debug.Log("Ya esta en el objetivo");
            OnPathFound?.Invoke();
            return null;
        }

		startNode.parent = startNode;

		if (!startNode.walkable) {
			startNode = grid.ClosestWalkableNode (startNode);
		}
		if (!targetNode.walkable) {
			targetNode = grid.ClosestWalkableNode (targetNode);
		}
		
		if (startNode.walkable && targetNode.walkable) {
			
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					sw.Stop();
					print ("Path found: " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour)+TurningCost(currentNode,neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else 
							openSet.UpdateItem(neighbour);
					}
				}
			}
		}

		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}

        OnPathFound?.Invoke();
        return waypoints;
	}

	
	int TurningCost(Node from, Node to) {
		/*
		Vector2 dirOld = new Vector2(from.gridX - from.parent.gridX, from.gridY - from.parent.gridY);
		Vector2 dirNew = new Vector2(to.gridX - from.gridX, to.gridY - from.gridY);
		if (dirNew == dirOld)
			return 0;
		else if (dirOld.x != 0 && dirOld.y != 0 && dirNew.x != 0 && dirNew.y != 0) {
			return 5;
		}
		else {
			return 10;
		}
		*/

		return 0;
	}
	
	Vector2[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

        path.Add(startNode);

        Vector2[] waypoints;
        if (grid.Diagonals)
        {
            waypoints = SimplifyPath(path);
        }
        else
        {
            waypoints = PathToVectorArray(path);
        }
		Array.Reverse(waypoints);
		return waypoints;
		
	}

    private Vector2[] PathToVectorArray(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        foreach(Node n in path)
        {
            waypoints.Add(n.worldPosition);
        }
        return waypoints.ToArray();
    }

    Vector2[] SimplifyPath(List<Node> path)
    {
		List<Vector2> waypoints = new List<Vector2>();
        waypoints.Add(path[0].worldPosition);
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}
	
	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
	
	
}
