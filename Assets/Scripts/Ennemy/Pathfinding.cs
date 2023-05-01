using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    private Queue queue;
    private Vector3 currentCell;

    private Vector3[] previousVerifyPath;

    private Vector3[] neighbors;

    // private int[,] neighbors = {
    //     {-1, 0}
    // };

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void GetPath(string[,] graph, Vector3 start, Vector3 destination) //array[x][z]
    {
        queue = new Queue();
        queue.Enqueue(start);
        currentCell = start;

        while (!queue.Equals(null)) // pas sûr
        {
            if(currentCell == destination) break;
                
            // foreach(string next in graph) 
            // {
                
            // }

            neighbors = GetNeighborsCells(graph);

            // for(int i = 0; i < neighbors.Length; i++) 
            // {
            //     if(neighbors[i])
            // }

            foreach(Vector3 neighbor in neighbors)
            {
                if(previousVerifyPath.Contains(neighbor)) //maybe an error with instance of class
                {
                    
                }
            }

        }
    }

    // public int[,] GetNeighborsCells(string[,] graph) 
    // {
    //     int[,] neighbors = new int[4,2];

    //     // logic to get my neighbors :
    //     //  - [0] => (current.x+1, current.z+0)
    //     //  - [1] => (current.x+0, current.z+1)
    //     //  - [2] => (current.x-1, current.z+0)
    //     //  - [3] => (current.x+0, current.z-1)

    //     if(currentCell.x <= 0) 
    //     {
    //         neighbors[0,0] = (int)currentCell.x-1;
    //     } else if (currentCell.x > graph.Length - 1) 
    //     {
    //         neighbors[2,0] = (int)currentCell.x-1;
    //     } else 
    //     {
    //         neighbors[0,0] = (int)currentCell.x+1;
    //         neighbors[2,0] = (int)currentCell.x-1;
    //     }

    //     if(currentCell.z <= 0)
    //     {
    //         neighbors[1,1] = (int)currentCell.z+1;
    //     } else if (currentCell.z > graph.Length - 1) 
    //     {
    //         neighbors[3,1] = (int)currentCell.z-1;
    //     } else
    //     {
    //         neighbors[1,1] = (int)currentCell.z+1;
    //         neighbors[3,1] = (int)currentCell.z-1;
    //     }

    //     return neighbors;
    // }

    public Vector3[] GetNeighborsCells(string[,] graph) 
    {
        // Vector3[] neighbors = new Vector3[4] {
        //     new Vector3(currentCell.x, currentCell.y, currentCell.z),
        //     new Vector3(currentCell.x, currentCell.y, currentCell.z), 
        //     new Vector3(currentCell.x, currentCell.y, currentCell.z), 
        //     new Vector3(currentCell.x, currentCell.y, currentCell.z), 
        // };

        Vector3[] neighbors = new Vector3[4];

        // logic to get my neighbors :
        //  - [0] => (current.x+1, current.z+0)
        //  - [1] => (current.x+0, current.z+1)
        //  - [2] => (current.x-1, current.z+0)
        //  - [3] => (current.x+0, current.z-1)

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
