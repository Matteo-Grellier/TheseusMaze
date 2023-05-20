using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f; // Look in Unity to change the speed !
    float elapsedTime = 0f;

    public bool isMoving = false;
    private bool hasDetectedPlayer = false;
    private bool isSearchingPlayer = false;

    private Pathfinding pathfinding;

    private Vector3 currentGraphPosition;
    private Vector3 nextGraphPosition;
    private Vector3 nextPosition = Vector3.zero;

    public Vector3 destination;

    private GameObject player;

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

        FindThePath();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if(GameManager.instance.mazeReference == null || !GameManager.instance.mazeReference.isDoneGenerating)
        {
            Debug.LogWarning("maze array is empty");
            return;
        }

        // if(hasDetectedPlayer && isSearchingPlayer)
        // {
        //     currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
        //     FindThePath();
        //     isSearchingPlayer = false;
        // } 


        // if(isSearchingPlayer && ConvertPositionToGraphPosition(transform.position) == destination)
        // {
        //     isSearchingPlayer = false;
        //     hasDetectedPlayer = false;   
        //     Debug.LogWarning("FALSY");
        // }

        if(!pathfinding.ispathFindingInProgress && destination != ConvertPositionToGraphPosition(transform.position))
            FindThePath();
       
        if(pathfinding.pathFound)
            MoveThroughPath();
               
        if(destination == ConvertPositionToGraphPosition(transform.position) || !pathfinding.isFindablePath)
        {
            // transform.position = ConvertGraphPositionToPosition(currentGraphPosition, transform.position.y);
            isMoving = false;
            pathfinding.ispathFindingInProgress = false;
            // pathfinding.ClearPathFindingData();

            // Search for a better solution ?
            isSearchingPlayer = false;
            hasDetectedPlayer = false; 

            currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
            destination = GetRandomVectorInMaze();

            // hasDetectedPlayer = false;
        }
    }

    private void FindThePath()
    {
        string[,] graph = GameManager.instance.mazeReference.mazeArray;
        // StartCoroutine(pathfinding.GraphSearch(graph, currentGraphPosition, destination)); // Search for a path in the graph
        pathfinding = new Pathfinding(graph, currentGraphPosition, destination);
        StartCoroutine(pathfinding.GraphSearch());
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

        if(nextPosition != transform.position)
            transform.forward = nextPosition-transform.position;

        if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition && nextGraphPosition != destination)
        {
            currentGraphPosition = nextGraphPosition;
            nextGraphPosition = pathfinding.GetNextDirection(currentGraphPosition);
            nextPosition = ConvertGraphPositionToPosition(nextGraphPosition, transform.position.y);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.fixedDeltaTime);
    }

    private Vector3 ConvertGraphPositionToPosition(Vector3 graphPosition, float yPosition)
    {
        return new Vector3(graphPosition.x, yPosition, graphPosition.z);
    }

    private Vector3 ConvertPositionToGraphPosition(Vector3 position)
    {
        return new Vector3(position.x, 0, position.z); // WARNING: TError here (because not 1,1) ?
    }

    private void OnTriggerStay(Collider other)
    {
        if(player != other.gameObject) return;

        RaycastHit hit;

        LayerMask entityLayerBit = 1 << 10;
        LayerMask wallLayerBit = 1 << 11;
        LayerMask layerMaskBits = wallLayerBit | entityLayerBit;

        if(Physics.Raycast(transform.position, player.transform.position-transform.position, out hit, Mathf.Infinity, layerMaskBits)) // Improve Performance with FixedUpdate
        {
            Debug.DrawRay(transform.position, (player.transform.position-transform.position), Color.red);
            Debug.LogWarning(hit.collider);
            
            if(hit.collider.gameObject == player) // TODO : Create a method for this, to DRY
            {
                if(!hasDetectedPlayer) 
                {
                    Debug.LogWarning("ARE YOU HERE ?");
                    hasDetectedPlayer = true;
                }

                if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition && hasDetectedPlayer && !isSearchingPlayer)
                {
                    isMoving = false;
                    Debug.LogWarning("GO CREATE A NEW PATH");
                    currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
                    destination = new Vector3((int)player.transform.position.x, 0, (int)player.transform.position.z);
                    FindThePath();
                    isSearchingPlayer = true;
                    return;
                }
            }
        }

        if(player.GetComponent<Player>().isTrapped)
        {
            if(!hasDetectedPlayer) 
            {
                Debug.LogWarning("ARE YOU HERE ?");
                hasDetectedPlayer = true;
            }

            if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition && hasDetectedPlayer && !isSearchingPlayer)
            {
                isMoving = false;
                Debug.LogWarning("GO CREATE A NEW PATH");
                currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
                destination = new Vector3((int)player.transform.position.x, 0, (int)player.transform.position.z);
                FindThePath();
                isSearchingPlayer = true;
                return;
            }
        } else {
            hasDetectedPlayer = false;
        }
    }
}
