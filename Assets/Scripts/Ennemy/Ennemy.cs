using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    // Start is called before the first frame update

    private Pathfinding pathfinding;
    void Start()
    {

        pathfinding = new Pathfinding();

        string[,] graph = {
            {"path", "path", "path", "wall", "path"},
            {"path", "wall", "path", "wall", "path"},
            {"path", "path", "path", "path", "path"},
            {"path", "path", "wall", "wall", "wall"},
            {"path", "path", "path", "path", "path"},
        };

        Vector3 destination = new Vector3(4, 0, 2);
        Vector3 start = new Vector3(2, 0, 4);

        StartCoroutine(pathfinding.GraphSearch(graph, start, destination));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
