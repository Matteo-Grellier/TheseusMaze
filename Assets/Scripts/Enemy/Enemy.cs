using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyAnimationStateController enemyAnimationController;

    [SerializeField] private AudioSource scaryAudioSource; 
    [SerializeField] private AudioSource screamerAudioSource; 
    [SerializeField] private AudioSource walkingAudioSource; 

    private bool isPlayedScreamerSound = false;

    public float speed = 2.5f; // Look in Unity to change the speed !

    private float visionAngle = 190f;
    private float visionDistance = 10f;
    private float distanceToCatch = 1f;
    private float distanceToListen = 3f;

    public bool isMoving = false;

    private bool isPlayerNearby = false;
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

        if(isMoving)
            walkingAudioSource.volume = 1;
        else
            walkingAudioSource.volume = 0;


        if(isPlayerNearby)
            HandleNearbyPlayer();

        if(!pathfinding.ispathFindingInProgress && destination != ConvertPositionToGraphPosition(transform.position))
            FindThePath();
       
        if(pathfinding.pathFound)
            MoveThroughPath();
               
        if(destination == ConvertPositionToGraphPosition(transform.position) || !pathfinding.isFindablePath)
        {
            isMoving = false;
            pathfinding.ispathFindingInProgress = false;

            // Search for a better solution ?
            isSearchingPlayer = false;
            hasDetectedPlayer = false; 

            if(!pathfinding.isFindablePath)
            {
                List<Vector3> neighbors = pathfinding.GetNeighborsCells(GameManager.instance.mazeReference.mazeArray);
                Vector3 newCurrentPosition = neighbors[Random.Range(0, neighbors.Count()-1)];
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.fixedDeltaTime);
            }

            currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
            destination = GetRandomVectorInMaze();

        }
    }

    private void FindThePath()
    {
        string[,] graph = GameManager.instance.mazeReference.mazeArray;
        pathfinding = new Pathfinding(graph, currentGraphPosition, destination);
        pathfinding.GraphSearch();
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

    private void OnTriggerEnter(Collider other)
    {
        if(player.gameObject != other.gameObject || GameManager.instance.isGameOver) return;
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(player.gameObject != other.gameObject || GameManager.instance.isGameOver) return;
            isPlayerNearby = false;
    }

    private void HandleNearbyPlayer()
    {
        HandleRaycast();

        bool playerMakeSounds = player.GetComponent<Rigidbody>().velocity != Vector3.zero && GetDistanceWithPlayer() <= distanceToListen;

        if(player.isTrapped || player.isWalkingOnGravel || playerMakeSounds)
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

            bool isHittingPlayer = hit.collider.gameObject == player.gameObject;
            Torchlight torchlight = player.GetComponentInChildren<Torchlight>();

            bool isPointedByTorchlight = GetIsPointedByTorchlight();
            bool isLookingAtPlayer = GetIsLookingAtPlayer();
            isCatchingPlayer = GetDistanceWithPlayer() <= distanceToCatch;

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
        return Mathf.Abs(player.transform.position.x - transform.position.x) + Mathf.Abs(player.transform.position.z - transform.position.z);
    }

    private void HandlePlayerDetection()
    {
        if(!hasDetectedPlayer) 
        {
            // Debug.LogWarning("ARE YOU HERE ?");
            hasDetectedPlayer = true;
        }

        if(ConvertPositionToGraphPosition(transform.position) == nextGraphPosition && hasDetectedPlayer && !isSearchingPlayer)
        {
            isMoving = false;
            currentGraphPosition = ConvertPositionToGraphPosition(transform.position);
            destination = new Vector3(Mathf.Round(player.transform.position.x), 0, Mathf.Round(player.transform.position.z));
            FindThePath();
            isSearchingPlayer = true;
            return;
        }
    }

    private void HandleGameOver()
    {
        Vector3 directionToPlayer = (player.transform.position-transform.position).normalized;
        Vector3 directionFromPlayer = (transform.position-player.transform.position).normalized;

        walkingAudioSource.volume = 0;
        scaryAudioSource.volume = 0;

        if(!isPlayedScreamerSound)
        {
            screamerAudioSource.PlayDelayed(1.2f);
            isPlayedScreamerSound = true;
        }

        player.transform.forward = new Vector3(directionFromPlayer.x, 0, directionFromPlayer.z);
        transform.forward = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
    }
}
