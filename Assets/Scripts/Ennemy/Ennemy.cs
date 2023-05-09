using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public float speed = 10f;

    private Pathfinding pathfinding;

    private Vector3 currentGraphPosition;
    private Vector3 nextGraphPosition;
    private Vector3 movementDirection = Vector3.zero;

    public Vector3 destination;

    void Start()
    {

        //Initialize nextGraphPosition to the transform position (to move with pathfinding in the Update method)
        nextGraphPosition = ConvertPositionToGraphPosition(transform.position);

        pathfinding = new Pathfinding();

        string[,] graph = {
            {"path", "path", "path", "path", "path"},
            {"path", "path", "path", "path", "path"},
            {"path", "path", "wall", "path", "path"},
            {"path", "path", "wall", "wall", "path"},
            {"path", "path", "path", "wall", "path"},
        };

        // string[,] graph = {
        //     {"path", "path", "path", "wall", "path"},
        //     {"path", "wall", "path", "wall", "path"},
        //     {"path", "path", "path", "path", "path"},
        //     {"path", "path", "wall", "wall", "wall"},
        //     {"path", "path", "path", "path", "path"},
        // };

        destination = new Vector3(4, 0, 2);
        currentGraphPosition = new Vector3(2, 0, 4);

        StartCoroutine(pathfinding.GraphSearch(graph, currentGraphPosition, destination));

    }

    void Update()
    {
        // if(nextGraphPosition == null && movementDirection == null)
        // {
        //     Debug.Log("here");
        //     nextGraphPosition = ConvertPositionToGraphPosition(transform.position);
        //     movementDirection = Vector3.zero;
        // }

        // nextGraphPosition = (nextGraphPosition != null) ? nextGraphPosition : ConvertPositionToGraphPosition(transform.position);
        // movementDirection = (movementDirection != null) ? movementDirection : Vector3.zero;

        if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition 
        && nextGraphPosition != destination)
        {
            currentGraphPosition = nextGraphPosition;
            Debug.Log("CURRENT GRAPH POSITION: " + currentGraphPosition);
            nextGraphPosition = pathfinding.GetNextDirection(currentGraphPosition);
            Debug.Log("NEXT DIRECTION" + nextGraphPosition);

            // movementDirection = ConvertGraphPositionToPosition(nextGraphPosition, transform.position.y) - ConvertGraphPositionToPosition(currentGraphPosition, transform.position.y);
            movementDirection = ConvertGraphPositionToPosition(nextGraphPosition, transform.position.y);
            Debug.Log(movementDirection);
        }
        
        Debug.Log("TRANSFORM POSITION" + transform.position);

        // transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
        transform.position = Vector3.MoveTowards(transform.position, movementDirection, speed * Time.deltaTime);
        // transform.position = Vector3.Lerp(transform.position, movementDirection, speed * Time.deltaTime);
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
