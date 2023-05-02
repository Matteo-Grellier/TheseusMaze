using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    private Queue queue;
    private Vector3 currentCell;

    // private Vector3[] previousVerifyPath;

    // Vector A -> Vector B : Key = the current cell (B) and Value = where the current cell came from (A).
    private Dictionary<Vector3, Vector3> positionNodes = new Dictionary<Vector3, Vector3>(); 

    private Vector3[] neighbors;

    public IEnumerator GetPath(string[,] graph, Vector3 start, Vector3 destination) //array[x][z]
    {
        queue = new Queue();
        queue.Enqueue(start);
        currentCell = start;

        while (!queue.Equals(null)) // pas sûr
        {

            currentCell = (Vector3) queue.Dequeue();
            Debug.Log(currentCell); 

            if(currentCell.x == destination.x && currentCell.z == destination.z) 
            {
                Debug.Log("WE FIND THE END OF THE WORLD");
                break;
            }

            neighbors = GetNeighborsCells(graph);

            foreach(Vector3 neighbor in neighbors)
            {
                Debug.Log(neighbor);

                if(!positionNodes.ContainsKey(neighbor)) //maybe an error with instance of class
                {
                    queue.Enqueue(neighbor);
                    positionNodes.Add(neighbor, currentCell);
                }

                // yield return null;

            }

            yield return null;
        }

        Debug.Log("oui");

        yield return null;
    }

    public Vector3[] GetNeighborsCells(string[,] graph) 
    {

        Vector3[] neighbors = new Vector3[4];


        if(currentCell.x <= 0) 
        {
            neighbors[0] += currentCell + Vector3.right;
        } else if (currentCell.x > graph.Length - 1) 
        {
            neighbors[2] += currentCell + Vector3.left;
        } else 
        {
            neighbors[0] += currentCell + Vector3.right;
            neighbors[2] += currentCell + Vector3.left;
        }

        if(currentCell.z <= 0)
        {
            neighbors[1] += currentCell + Vector3.forward;
        } else if (currentCell.z > graph.Length - 1) 
        {
            neighbors[3] += currentCell + Vector3.back;
        } else
        {
            neighbors[1] += currentCell + Vector3.forward;
            neighbors[3] += currentCell + Vector3.back;
        }

        return neighbors;
    }
}
