using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public float speed = 10f;

    private Pathfinding pathfinding;

    private Vector3 currentGraphPosition;
    private Vector3 nextGraphPosition;
    private Vector3 movementDirection = Vector3.zero;

    public Vector3 destination;

    void Start() // Now we can use Start because the Instance is created in gameManager when the maze is generated !
    {

        if(GameManager.instance.mazeReference.mazeArray != null)
        {
            currentGraphPosition = GetRandomVectorInMaze();
            transform.position = new Vector3(currentGraphPosition.x, transform.position.y, currentGraphPosition.z);
            destination = GetRandomVectorInMaze();
        }

        // destination = new Vector3(4, 0, 2);

        //Initialize nextGraphPosition to the transform position (to move with pathfinding in the Update method)
        nextGraphPosition = ConvertPositionToGraphPosition(transform.position);

        pathfinding = new Pathfinding();

        // string[,] graph = {
        //     {"path", "path", "path", "path", "path"},
        //     {"path", "path", "path", "path", "path"},
        //     {"path", "path", "wall", "path", "path"},
        //     {"path", "path", "wall", "wall", "path"},
        //     {"path", "path", "path", "wall", "path"},
        // };

        // string[,] graph = {
        //     {"path", "path", "path", "wall", "path"},
        //     {"path", "wall", "path", "wall", "path"},
        //     {"path", "path", "path", "path", "path"},
        //     {"path", "path", "wall", "wall", "wall"},
        //     {"path", "path", "path", "path", "path"},
        // };

        // string[,] graph = GameManager.instance.mazeReference.mazeArray;

        // StartCoroutine(pathfinding.GraphSearch(graph, currentGraphPosition, destination));

    }

    void FixedUpdate()
    {
        if(GameManager.instance.mazeReference == null || !GameManager.instance.mazeReference.isDoneGenerating)
        {
            Debug.Log("maze array is empty");
            return;
        }

        if(!pathfinding.ispathFindingInProgress && destination != ConvertPositionToGraphPosition(transform.position))
        {
            string[,] graph = GameManager.instance.mazeReference.mazeArray;
            Debug.Log(graph[2, 4]);
            StartCoroutine(pathfinding.GraphSearch(graph, currentGraphPosition, destination)); // Search for a path in the graph
        }
       
        if(pathfinding.pathFound)
            MoveThroughPath();
        
        if(destination == ConvertPositionToGraphPosition(transform.position))
            pathfinding.ispathFindingInProgress = false;
    }

    private Vector3 GetRandomVectorInMaze()
    {
        int randomX = Random.Range(0, GameManager.instance.mazeReference.mazeArray.GetLength(0)-1);
        int randomZ = Random.Range(0, GameManager.instance.mazeReference.mazeArray.GetLength(1)-1);

        while(GameManager.instance.mazeReference.mazeArray[randomX, randomZ] == "wall")
        {
            randomX = Random.Range(0, GameManager.instance.mazeReference.mazeArray.GetLength(0)-1);
            randomZ = Random.Range(0, GameManager.instance.mazeReference.mazeArray.GetLength(1)-1);
            Debug.Log("(" + randomX + ", " + randomZ + ")");
        }

        return new Vector3(randomX, 0, randomZ);
    }

    private void MoveThroughPath()
    {
        if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition && nextGraphPosition != destination)
        {
            currentGraphPosition = nextGraphPosition;
            nextGraphPosition = pathfinding.GetNextDirection(currentGraphPosition);
            movementDirection = ConvertGraphPositionToPosition(nextGraphPosition, transform.position.y);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, movementDirection, speed * Time.deltaTime);
    }

    private Vector3 ConvertGraphPositionToPosition(Vector3 graphPosition, float yPosition)
    {
        return new Vector3(graphPosition.x, yPosition, graphPosition.z);
    }

    private Vector3 ConvertPositionToGraphPosition(Vector3 position)
    {
        return new Vector3(position.x, 0, position.z);
    }
}
