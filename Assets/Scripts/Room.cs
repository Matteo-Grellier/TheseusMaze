using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject casePrefab;
    public int roomSize;
    public int roomID = 0;
    public bool isAKeyRoom = false;

    private int caseIteration = 0;
    private int caseColumn = 0;
    private int caseRow = 0;
    private bool keyIsSet = false;

    private bool isTrapShown = false;
    private bool isWallShown = false;
    private bool isGravelShown = false;
    private bool isMudShown = false;
    private bool isElevator = false;
    private bool isKey = false;

    private bool isAfterGenerationCodeExecuted = false;

    private Vector3 pos;
    private Material roomMaterial; // his size is set in the Maze
    public string[ , ] roomArray; // his size is set in the Maze
    public GameObject[ , ] casesArray; // his size is set in the Maze (when in random, otherwise it's null)
    public Maze mazeReference; // set in the Maze

    // is set in the Maze that creates it, but only if the Maze is generating randomely, otherwise it's empty
    public Maze.RoomObject room;

    private void Awake()
    {
        pos = gameObject.transform.position;
        roomMaterial = new Material(Shader.Find("Specular"));
        roomMaterial.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void Update()
    {
        // cases
        if (caseIteration < roomSize * roomSize)
        {
            // + 0.5f otherwise it will be slightly offcentered, -roomSize/2.0f to place it on the left down of the cube wich is CENTERED on 0,0,0 (same for the Z),
            // divide by 2.0f to get a float (not a rounded int), (-caseRow) beacause caseRow is positive and should be place under
            Vector3 newCasePosition = new Vector3((pos.x + 0.5f) - (roomSize/2.0f) + (1 * caseColumn), 0.55f, (pos.z + 0.5f) - (roomSize/2.0f) + caseRow );
            GameObject newCase = Instantiate(casePrefab, newCasePosition , Quaternion.Euler(new Vector3(0, 0, 0)));
            newCase.transform.parent = gameObject.transform; //set the case as child of the room
            newCase.GetComponent<Case>().caseId = caseIteration;
            newCase.GetComponent<Case>().RoomId = roomID;
            newCase.GetComponent<Case>().caseMazeReference = mazeReference;
            Case newCaseScript = newCase.gameObject.GetComponent<Case>();

            bool isWallShown;
            if (!GameManager.instance.isEditingNewlyCreatedMap && !GameManager.instance.isRandomlyGenerated)  // if not in editing new map and not randomelygenerated
            {
                isWallShown = false;
                isTrapShown = false;
                isGravelShown = false;
                isMudShown = false;
                isTrapShown = false;
                isElevator = false;
                isKey = false;
                switch (room.cases[caseIteration].state)
                { 
                    case "wall" :
                        isWallShown = true;
                        roomArray[caseColumn,caseRow] = "wall";
                        break;
                    case "path" :
                        isWallShown = false;
                        roomArray[caseColumn,caseRow] = "path";
                        break;
                    case "trap" :
                        isWallShown = false;
                        isTrapShown = true;
                        roomArray[caseColumn,caseRow] = "trap";
                        break;
                    case "gravel" :
                        isWallShown = false;
                        isGravelShown = true;
                        roomArray[caseColumn,caseRow] = "gravel";
                        break;
                    case "mud" :
                        isWallShown = false;
                        isMudShown = true;
                        roomArray[caseColumn,caseRow] = "mud";
                        break;
                    case "elevator" :
                        isWallShown = false;
                        isElevator = true;
                        roomArray[caseColumn,caseRow] = "elevator";
                        break;
                    case "key" :
                        isWallShown = false;
                        isKey = true;
                        roomArray[caseColumn,caseRow] = "key";
                        break;

                    default :
                        isWallShown = false;
                        // Debug.LogWarning("What the fuck is a " +  room.cases[caseIteration].state + " ???? I put a \"path\" instead ");
                        roomArray[caseColumn,caseRow] = "path";
                        break;
                }
            }
            else if (GameManager.instance.isEditMode && GameManager.instance.isEditingNewlyCreatedMap) // if editing a newly created map set all walls to path
            {
                isWallShown = false;
                roomArray[caseColumn,caseRow] = "path";
                Maze.CaseObject newCaseObject = new Maze.CaseObject();
                newCaseObject.cellid = caseIteration;
                newCaseObject.state = "path";
                mazeReference.maze.rooms[roomID].cases.Add(newCaseObject);
            }
            else // random
            {
                isWallShown = true;
                roomArray[caseColumn,caseRow] = "wall";
                Maze.CaseObject newCaseObject = new Maze.CaseObject();
                newCaseObject.cellid = caseIteration;
                newCaseObject.state = "wall";
                casesArray[caseColumn,caseRow] = newCase;
                // Debug.Log( "room " + roomID + " : " + "caseColumn : " + caseColumn + " caseRow : " + caseRow + " value :" + roomArray[caseColumn,caseRow]);
            }

            newCaseScript.wallObject.SetActive(isWallShown);
            newCaseScript.trapObject.SetActive(isTrapShown);
            newCaseScript.gravelObject.SetActive(isGravelShown);
            newCaseScript.mudObject.SetActive(isMudShown);
            newCaseScript.elevatorObject.SetActive(isElevator);
            newCaseScript.keyObject.SetActive(isKey);

            if (room.isEndRoom) // if the room is an end room
                newCaseScript.debugCase.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black); // set material to black
            else
                newCaseScript.debugCase.GetComponent<MeshRenderer>().material = roomMaterial; // otherwise random color

            caseIteration++;
            caseColumn++;
            if (caseIteration != 0 && caseColumn == roomSize)
            {
                caseRow++;
                caseColumn = 0;
            }
        }
        else
        {
            if ( !isAfterGenerationCodeExecuted )
            {
                gameObject.transform.parent.GetComponent<Maze>().RoomsStillGenerating--;
                //Debug.Log("room " + roomID + " : " + roomArray[0,0] + " : " + roomArray[1,0] + " : " + roomArray[0,1] + " : " + roomArray[1,1]);
                isAfterGenerationCodeExecuted = true;
                StartCoroutine(RoomPathGeneration());
            }
        }
    }
    // if (isAKeyRoom)
    //     if (!keyIsSet && randomNumber == 1 || !keyIsSet && caseIteration == (roomSize*roomSize) - 1)// if is 1 or is not set
    //         Debug.Log("<color=yellow>[key] Key Case is Set : " + caseIteration +" </color>");
    //         keyIsSet = true;
    //         isKey = true;
    //         roomArray[caseColumn,caseRow] = "key";
    // if (room.isEndRoom == true)
    //     if (caseIteration ==  (roomSize*roomSize) / 2) // to set it in the middle of the room
    //         isElevator = true;
    //         roomArray[caseColumn,caseRow] = "elevator";
    // if (!isKey && !room.isEndRoom) // if it's a key case or a end room, no need for all this
    //      sWallShown = true;
    //          roomArray[caseColumn,caseRow] = "wall";
    //      isTrapShown = true;
    //          roomArray[caseColumn,caseRow] = "trap";
    //      isGravelShown = true;
    //          roomArray[caseColumn,caseRow] = "gravel";
    //      isMudShown = true;
    //          roomArray[caseColumn,caseRow] = "mud";
    //      roomArray[caseColumn,caseRow] = "path";
    private Queue<Vector3> queue;
    private Queue<Vector3> directionQueue;
    private IEnumerator RoomPathGeneration()
    {
        queue = new Queue<Vector3>();
        directionQueue = new Queue<Vector3>();
        int halfRoomSize = roomSize/ 2;
        int maxRoomSize = roomSize - 1;
        // up
        casesArray[halfRoomSize, maxRoomSize].GetComponent<Case>().wallObject.SetActive(false);
        queue.Enqueue(new Vector3(halfRoomSize, 0, maxRoomSize));
        directionQueue.Enqueue( Vector3.back);
        // down
        casesArray[halfRoomSize, 0].GetComponent<Case>().wallObject.SetActive(false);
        queue.Enqueue(new Vector3(halfRoomSize, 0, 0));
        directionQueue.Enqueue(Vector3.forward);
        // left
        casesArray[0, halfRoomSize].GetComponent<Case>().wallObject.SetActive(false);
        queue.Enqueue(new Vector3(0, 0, halfRoomSize));
        directionQueue.Enqueue(Vector3.right);
        // right
        casesArray[maxRoomSize, halfRoomSize].GetComponent<Case>().wallObject.SetActive(false);
        queue.Enqueue(new Vector3(maxRoomSize, 0, halfRoomSize));
        directionQueue.Enqueue(Vector3.left);

        Vector3 newCaseVector;
        while (queue.Count != 0)
        {
            Vector3 currentCase = (Vector3) queue.Dequeue();
            Vector3 currentDirection = (Vector3) directionQueue.Dequeue();
            int randomNumber = Random.Range(1,4); // between 1 and 3
            switch (randomNumber)
            {
                case 1 : // going forward
                    newCaseVector = currentCase + currentDirection;
                    Debug.Log("<color=red>currentCase = [" + currentCase.x + "," + currentCase.z + "]</color> + <color=purple>currentDirection = [" + currentDirection.x + "," + currentDirection.z + "]</color> = <color=green>newCaseVector = [" + newCaseVector.x + "," + newCaseVector.z + "]</color>");
                    if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if inside the bounds of the room
                    {
                        // insérer ici un code pour vérifier si c'est pas un bordure, sinon ça prépare un tournant et ça remet dans la queue
                        casesArray[(int)newCaseVector.x, (int)newCaseVector.z].GetComponent<Case>().wallObject.SetActive(false);
                        queue.Enqueue(newCaseVector);
                        directionQueue.Enqueue(currentDirection);
                    }
                    else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0)
                    {
                        StopCoroutine(RoomPathGeneration());
                    }
                    break;
                case 2 : // turning on one side
                    if(currentDirection == Vector3.back || currentDirection == Vector3.forward)
                        currentDirection = (Random.Range(0,2) == 0) ? Vector3.right : Vector3.left;
                    // insérer ici un code pour vérifier si c'est pas un coins, sinon ça ne fait pas et ça remet dans la queue
                    newCaseVector = currentCase + currentDirection;
                    Debug.Log("<color=red>currentCase = [" + currentCase.x + "," + currentCase.z + "]</color> + <color=purple>currentDirection = [" + currentDirection.x + "," + currentDirection.z + "]</color> = <color=green>newCaseVector = [" + newCaseVector.x + "," + newCaseVector.z + "]</color>");
                    if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if inside the bounds of the room
                    {
                        casesArray[(int)newCaseVector.x, (int)newCaseVector.z].GetComponent<Case>().wallObject.SetActive(false);
                        queue.Enqueue(newCaseVector);
                        directionQueue.Enqueue(currentDirection);
                    }
                    else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0)
                    {
                        StopCoroutine(RoomPathGeneration());
                    }
                    break;
                case 3 : // dissociate into two path
                    newCaseVector = currentCase + currentDirection;
                    Debug.Log("<color=red>currentCase = [" + currentCase.x + "," + currentCase.z + "]</color> + <color=purple>currentDirection = [" + currentDirection.x + "," + currentDirection.z + "]</color> = <color=green>newCaseVector = [" + newCaseVector.x + "," + newCaseVector.z + "]</color>");
                    if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if inside the bounds of the room
                    {
                        // one go forward
                        casesArray[(int)newCaseVector.x, (int)newCaseVector.z].GetComponent<Case>().wallObject.SetActive(false);
                        queue.Enqueue(newCaseVector);
                        directionQueue.Enqueue(currentDirection);
                        // one go to side
                        if(currentDirection == Vector3.back || currentDirection == Vector3.forward)
                            currentDirection = (Random.Range(0,2) == 0) ? Vector3.right : Vector3.left;
                        // insérer ici un code pour vérifier si c'est pas un coins, sinon ça ne fait pas et ça remet dans la queue
                        newCaseVector = currentCase + currentDirection;
                        casesArray[(int)newCaseVector.x, (int)newCaseVector.z].GetComponent<Case>().wallObject.SetActive(false);
                        queue.Enqueue(newCaseVector);
                        directionQueue.Enqueue(currentDirection);
                    }
                    else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0)
                    {
                        StopCoroutine(RoomPathGeneration());
                    }
                    break;
            }
            // yield return null;
            yield return new WaitForSeconds(1);
        }

        yield return null;
    }
}
