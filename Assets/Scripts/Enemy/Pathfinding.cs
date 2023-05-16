using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class Pathfinding
{
    /*
    TODO : IMPROVE CODE WITH CONSTRUCTOR => when we create a pathfinding, we use a constructor with graph, destination and start.
    So we can call all methods for the pathfinding (GraphSearch, CreatePath etc...). And maybe we can use the Coroutine in the constructor.
    */

    private PriorityQueue<Vector3, int> queue;
    private Dictionary<Vector3, int> movementCosts;
    private Vector3 currentCell;

    // private Vector3[] previousVerifyPath;

    // Vector A -> Vector B : Key = the current cell (B) and Value = where the current cell came from (A).
    private Dictionary<Vector3, Vector3> verifiedNodes = new Dictionary<Vector3, Vector3>(); // Nodes or Edges ?

    private List<Vector3> neighbors;

    public bool pathFound = false;
    public bool ispathFindingInProgress = false;
    public Dictionary<Vector3, Vector3> pathNodes;

    public IEnumerator GraphSearch(string[,] graph, Vector3 start, Vector3 destination) //array[x][z]
    {
        pathFound = false;
        ispathFindingInProgress = true;

        // queue = new Queue();
        queue = new PriorityQueue<Vector3, int>();
        movementCosts = new Dictionary<Vector3, int>();

        queue.Enqueue(start, 0);
        currentCell = start;
        movementCosts.Add(start, 0);
        verifiedNodes.Add(start, start);

        while (queue.Count != 0)
        {

            currentCell = (Vector3) queue.Dequeue();

            if(currentCell.x == destination.x && currentCell.z == destination.z) 
            {
                break;
            }

            neighbors = GetNeighborsCells(graph);

            foreach(Vector3 neighbor in neighbors)
            {
                int currentCost = movementCosts[currentCell] + GetCurrentNodeCost(graph, neighbor);
                
                if(!movementCosts.ContainsKey(neighbor) || currentCost < movementCosts[neighbor]) //maybe an error with instance of class ?
                {
                    if(movementCosts.ContainsKey(neighbor))
                        movementCosts[neighbor] = currentCost;
                    else
                        movementCosts.Add(neighbor, currentCost);
                        
                    int priority = currentCost + GetEstimatedDistance(destination, neighbor);
                    queue.Enqueue(neighbor, priority);

                    if(verifiedNodes.ContainsKey(neighbor)) 
                        verifiedNodes[neighbor] = currentCell;
                    else
                         verifiedNodes.Add(neighbor, currentCell);
                }
            }

            yield return null;
        }

        CreatePath(start, destination);

        yield return null;
    }

    private List<Vector3> GetNeighborsCells(string[,] graph) 
    {

        List<Vector3> neighbors = new List<Vector3>();

        int graphZLength = graph.GetLength(0);
        int graphXLength = graph.GetLength(1);

        //TODO : Refactor the code to be more readable.
        if(currentCell.x-1 >= 0 && graph[(int)(currentCell.x - 1), (int)currentCell.z] != "wall" )
        {
            neighbors.Add(currentCell + Vector3.left);
        }

        if(currentCell.x+1 < graphXLength && graph[(int)(currentCell.x + 1), (int)currentCell.z] != "wall")
        {
            neighbors.Add(currentCell + Vector3.right);
        }

        if(currentCell.z-1 >= 0 && graph[(int)currentCell.x, (int)(currentCell.z - 1)] != "wall")
        {
            neighbors.Add(currentCell + Vector3.back);
        }

        if(currentCell.z+1 < graphZLength && graph[(int)currentCell.x, (int)currentCell.z + 1] != "wall")
        {
            neighbors.Add(currentCell + Vector3.forward);
        }

        return neighbors;
    }

    private int GetCurrentNodeCost(string[,] graph, Vector3 neighbor)
    {
        switch(graph[(int)neighbor.x, (int)neighbor.z])
        {
            case "mud":
                return 3;
            case "trap":
                return 6;
            default:
                return 0;
        }

    }

    private int GetEstimatedDistance(Vector3 firstPosition, Vector3 secondPosition)
    {
        return Math.Abs((int)firstPosition.x - (int)secondPosition.x) + Math.Abs((int)firstPosition.y - (int)secondPosition.y);
    }

    private void CreatePath(Vector3 start, Vector3 destination)
    {

        pathNodes = new Dictionary<Vector3, Vector3>();

        pathNodes.Add(destination, verifiedNodes[destination]);

        Vector3 currentNode = destination;

        while(currentNode != start)
        {
            // Vector A -> Vector B
            Debug.Log(currentNode + "-->" + verifiedNodes[currentNode]);
            currentNode = verifiedNodes[currentNode];
            pathNodes.Add(currentNode, verifiedNodes[currentNode]);
        }

        pathFound = true;
    }

    public Vector3 GetNextDirection(Vector3 currentPosition)
    {
        if(!pathFound || !pathNodes.ContainsKey(currentPosition)) return currentPosition;

        return pathNodes.First(x => x.Value == currentPosition).Key;
    }

    public void ClearPathFindingData()
    {
        pathNodes.Clear();
        queue.Clear();
        verifiedNodes.Clear();
        neighbors.Clear();
    }
}
