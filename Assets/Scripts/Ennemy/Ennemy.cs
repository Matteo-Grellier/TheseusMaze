using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    // Start is called before the first frame update

    private Pathfinding pathfinding;

    private Vector3 currentGraphPosition;
    public Vector3 destination;

    void Start()
    {

        pathfinding = new Pathfinding();

        // string[,] graph = {
        //     {"path", "path", "path", "path", "path"},
        //     {"path", "path", "path", "path", "path"},
        //     {"path", "path", "wall", "path", "path"},
        //     {"path", "path", "wall", "wall", "path"},
        //     {"path", "path", "path", "wall", "path"},
        // };

        string[,] graph = {
            {"path", "path", "path", "wall", "path"},
            {"path", "wall", "path", "wall", "path"},
            {"path", "path", "path", "path", "path"},
            {"path", "path", "wall", "wall", "wall"},
            {"path", "path", "path", "path", "path"},
        };

        destination = new Vector3(4, 0, 2);
        currentGraphPosition = new Vector3(2, 0, 4);

        StartCoroutine(pathfinding.GraphSearch(graph, currentGraphPosition, destination));

    }

    // Update is called once per frame
    void Update()
    {
        currentGraphPosition = pathfinding.GetNextDirection(currentGraphPosition);
        Debug.Log("NEXT DIRECTION" + currentGraphPosition);
    }
}
