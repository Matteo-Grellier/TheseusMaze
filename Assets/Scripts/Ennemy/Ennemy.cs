using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public float speed = 10f;

    public bool isMoving = false;
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

        //Initialize nextGraphPosition to the transform position (to move with pathfinding in the Update method)
        nextGraphPosition = ConvertPositionToGraphPosition(transform.position);

        pathfinding = new Pathfinding();
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
        {
            isMoving = false;
            pathfinding.ispathFindingInProgress = false;
            pathfinding.ClearPathFindingData();

            currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
            destination = GetRandomVectorInMaze();
        }
    }

    private Vector3 GetRandomVectorInMaze()
    {
        int randomX = Random.Range(0, GameManager.instance.mazeReference.mazeArray.GetLength(0)-1);

        string[] rowsForRandomZ = Enumerable.Range(0, GameManager.instance.mazeReference.mazeArray.GetLength(1))
                .Select(x => GameManager.instance.mazeReference.mazeArray[randomX, x])
                .ToArray();
        
        IEnumerable<int> allValidZIndex = rowsForRandomZ
        .Select((value, index) => new { value, index })
        .Where(item => item.value != "wall")
        .Select(item => item.index);

        int randomZ = allValidZIndex.ElementAt(Random.Range(0, allValidZIndex.Count()-1));

        return new Vector3(randomX, 0, randomZ);
    }

    private void MoveThroughPath()
    {
        isMoving = true;

        if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition && nextGraphPosition != destination)
        {
            currentGraphPosition = nextGraphPosition;
            nextGraphPosition = pathfinding.GetNextDirection(currentGraphPosition);
            movementDirection = ConvertGraphPositionToPosition(nextGraphPosition, transform.position.y);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, movementDirection, speed * Time.deltaTime);
        
        if(movementDirection != transform.position)
            transform.forward = movementDirection-transform.position;
    }

    private Vector3 ConvertGraphPositionToPosition(Vector3 graphPosition, float yPosition)
    {
        return new Vector3(graphPosition.x, yPosition, graphPosition.z);
    }

    private Vector3 ConvertPositionToGraphPosition(Vector3 position)
    {
        return new Vector3((int)position.x, 0, (int)position.z); // WARNING: This can be create strange movement ?
    }
}
