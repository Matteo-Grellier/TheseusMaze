using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    // Start is called before the first frame update

    public Pathfinding pathfinding = new Pathfinding();
    void Start()
    {

        string[,] graph = {
            {"path", "path", "path", "wall", "path"},
            {"path", "wall", "path", "wall", "path"},
            {"path", "path", "path", "path", "path"},
            {"path", "path", "wall", "wall", "wall"},
            {"path", "path", "path", "path", "path"},
        };

        Vector3 destination = new Vector3(4, 0, 2);
        Vector3 start = new Vector3(0, 0, 0);

        pathfinding.GetPath(graph, start, destination);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
