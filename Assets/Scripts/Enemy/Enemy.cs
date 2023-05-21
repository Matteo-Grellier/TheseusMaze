using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f; // Look in Unity to change the speed !

    private float visionAngle = 190f;
    private float visionDistance = 10f;
    private float distanceToCatch = 1f;

    public bool isMoving = false;
    private bool hasDetectedPlayer = false;
    private bool isSearchingPlayer = false;

    private bool isCatchingPlayer = true;

    private Pathfinding pathfinding;

    private Vector3 currentGraphPosition;
    private Vector3 nextGraphPosition;
    private Vector3 nextPosition = Vector3.zero;

    public Vector3 destination;

    private Player player;

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

        player = GameManager.instance.player;
    }

    void FixedUpdate()
    {
        if(GameManager.instance.mazeReference == null || !GameManager.instance.mazeReference.isDoneGenerating)
        {
            Debug.LogWarning("maze array is empty");
            return;
        }

        if(GameManager.instance.isGameOver) 
        {
            HandleGameOver();
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
            transform.forward = (nextPosition-transform.position).normalized;

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
        if(player.gameObject != other.gameObject || GameManager.instance.isGameOver) return;

        RaycastHit hit;

        LayerMask entityLayerBit = 1 << 12;
        LayerMask wallLayerBit = 1 << 13;
        LayerMask layerMaskBits = wallLayerBit | entityLayerBit;

        HandleRaycast();

        if(Physics.Raycast(transform.position, player.transform.position-transform.position, out hit, Mathf.Infinity, layerMaskBits)) // Improve Performance with FixedUpdate
        {
            Debug.DrawRay(transform.position, (player.transform.position-transform.position), Color.red);
            // Debug.LogWarning(hit.collider);
            
            if(hit.collider.gameObject == player.gameObject 
            && player.GetComponentInChildren<Torchlight>().isLightUp 
            && player.transform.forward == (player.transform.position-transform.position).normalized)
            {
                HandlePlayerDetection();
            }
        }

        if(player.isTrapped || player.isWalkingOnGravel)
        {
            HandlePlayerDetection();
        } 
        else {
            hasDetectedPlayer = false;
        }
    }

    private void HandleRaycast()
    {
        RaycastHit hit;

        LayerMask entityLayerBit = 1 << 12;
        LayerMask wallLayerBit = 1 << 13;
        LayerMask layerMaskBits = wallLayerBit | entityLayerBit;

        bool isRaycasting = Physics.Raycast(transform.position, player.transform.position-transform.position, out hit, Mathf.Infinity, layerMaskBits);
        
        if(isRaycasting) // Improve Performance with FixedUpdate
        {
            Debug.DrawRay(transform.position, (player.transform.position-transform.position), Color.red);
            // Debug.LogWarning(hit.collider);

            bool isHittingPlayer = hit.collider.gameObject == player.gameObject;
            Torchlight torchlight = player.GetComponentInChildren<Torchlight>();

            bool isPointedByTorchlight = GetIsPointedByTorchlight();
            bool isLookingAtPlayer = GetIsLookingAtPlayer();
            isCatchingPlayer = GetDistanceWithPlayer() <= distanceToCatch;

            // Debug.LogWarning(isLookingAtPlayer);

            if(isHittingPlayer && isCatchingPlayer)
            {
                GameManager.instance.isGameOver = true;
            } else if(isHittingPlayer && (torchlight.isLightUp && isPointedByTorchlight || isLookingAtPlayer))
            {
                HandlePlayerDetection();
            }
        }
    }

    private bool GetIsPointedByTorchlight()
    {
        Vector3 directionFromPlayer = (transform.position-player.transform.position).normalized;
        Torchlight torchlight = player.GetComponentInChildren<Torchlight>();
        
        if(Vector3.Angle(player.transform.forward, directionFromPlayer) < torchlight.GetComponent<Light>().spotAngle / 2)
        {
            return true;
        } 

        return false;
    }

    private bool GetIsLookingAtPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position-transform.position).normalized;
        
        float distanceWithPlayer = GetDistanceWithPlayer();
        
        if((Vector3.Angle(transform.forward, directionToPlayer) < visionAngle / 2) && distanceWithPlayer <= visionDistance)
        {
            return true;
        } 

        return false;
    }

    private float GetDistanceWithPlayer()
    {
        return Mathf.Abs(player.transform.position.x - transform.position.x) + Mathf.Abs(player.transform.position.y - transform.position.y);
    }

    private void HandlePlayerDetection()
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
            destination = new Vector3(Mathf.Round(player.transform.position.x), 0, Mathf.Round(player.transform.position.z));
            FindThePath();
            isSearchingPlayer = true;
            return;
        }
    }

    private void HandleGameOver()
    {
        Debug.LogWarning("I'm here");
        // player.transform.LookAt(transform, Vector3.forward);

        Vector3 distanceToPlayer = (player.transform.position-transform.position).normalized;
        Vector3 distanceFromPlayer = (transform.position-player.transform.position).normalized;

        player.transform.forward = new Vector3(distanceFromPlayer.x, 0, distanceFromPlayer.z);
        transform.forward = new Vector3(distanceToPlayer.x, 0, distanceToPlayer.z);
    }
}
