using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    /*
    TODO : IMPROVE CODE WITH CONSTRUCTOR => when we create a pathfinding, we use a constructor with graph, destination and start.
    So we can call all methods for the pathfinding (GraphSearch, CreatePath etc...). And maybe we can use the Coroutine in the constructor.
    */

    private Queue queue;
    private Vector3 currentCell;

    // private Vector3[] previousVerifyPath;

    // Vector A -> Vector B : Key = the current cell (B) and Value = where the current cell came from (A).
    private Dictionary<Vector3, Vector3> positionNodes = new Dictionary<Vector3, Vector3>(); 

    private List<Vector3> neighbors;

    public IEnumerator GraphSearch(string[,] graph, Vector3 start, Vector3 destination) //array[x][z]
    {
        queue = new Queue();
        queue.Enqueue(start);
        currentCell = start;

        while (!queue.Equals(null)) // pas sûr
        {

            currentCell = (Vector3) queue.Dequeue();
            Debug.Log("currentCell" + currentCell); 

            if(currentCell.x == destination.x && currentCell.z == destination.z) 
            {
                // Debug.Log("WE FIND THE END OF THE WORLD");
                break;
            }

            neighbors = GetNeighborsCells(graph);

            foreach(Vector3 neighbor in neighbors)
            {
                // Debug.Log("neighbor of " + currentCell + "is " + neighbor);

                if(!positionNodes.ContainsKey(neighbor)) //maybe an error with instance of class ?
                {
                    queue.Enqueue(neighbor);
                    positionNodes.Add(neighbor, currentCell);
                }

                // yield return null;

            }

            yield return null;
        }

        // Debug.Log("oui");

        CreatePath(graph, start, destination);

        yield return null;
    }

    private List<Vector3> GetNeighborsCells(string[,] graph) 
    {

        List<Vector3> neighbors = new List<Vector3>();

        int graphZLength = graph.GetLength(0);
        int graphXLength = graph.GetLength(1);

        // if(currentCell.x <= 0) // || graph[(int)(currentCell.x + Vector3.left.x), (int)currentCell.z] == "wall" 
        // {
        //     neighbors.Add(currentCell + Vector3.right);
        // } else if (currentCell.x >= graphXLength - 1) // || graph[(int)(currentCell.x + Vector3.right.x), (int)currentCell.z] == "wall"
        // {
        //     neighbors.Add(currentCell + Vector3.left);
        // } else 
        // {
        //     neighbors.Add(currentCell + Vector3.right);
        //     neighbors.Add(currentCell + Vector3.left);
        // }

        // if(currentCell.z <= 0)
        // {
        //     neighbors.Add(currentCell + Vector3.forward);
        // } else if (currentCell.z >= graphZLength - 1) 
        // {
        //     neighbors.Add(currentCell + Vector3.back);
        // } else
        // {
        //     neighbors.Add(currentCell + Vector3.forward);
        //     neighbors.Add(currentCell + Vector3.back);
        // }

        //TODO : Refactor the code to be more readable.
        if(currentCell.x != 0 && graph[(int)(currentCell.x - 1), (int)currentCell.z] != "wall" )
        {
            neighbors.Add(currentCell + Vector3.left);
        }

        if(currentCell.x != graphXLength - 1 && graph[(int)(currentCell.x + 1), (int)currentCell.z] != "wall")
        {
            neighbors.Add(currentCell + Vector3.right);
        }

        if(currentCell.z != 0 && graph[(int)currentCell.x, (int)(currentCell.z - 1)] != "wall")
        {
            neighbors.Add(currentCell + Vector3.back);
        }

        if(currentCell.z != graphZLength - 1 && graph[(int)currentCell.x, (int)currentCell.z + 1] != "wall")
        {
            neighbors.Add(currentCell + Vector3.forward);
        }

        return neighbors;
    }

    private void CreatePath(string[,] graph, Vector3 start, Vector3 destination)
    {

    }
}
