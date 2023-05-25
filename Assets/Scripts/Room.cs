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
                if(roomID == 0)
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
        // // down
        // casesArray[halfRoomSize, 0].GetComponent<Case>().wallObject.SetActive(false);
        // queue.Enqueue(new Vector3(halfRoomSize, 0, 0));
        // directionQueue.Enqueue(Vector3.forward);
        // // left
        // casesArray[0, halfRoomSize].GetComponent<Case>().wallObject.SetActive(false);
        // queue.Enqueue(new Vector3(0, 0, halfRoomSize));
        // directionQueue.Enqueue(Vector3.right);
        // // right
        // casesArray[maxRoomSize, halfRoomSize].GetComponent<Case>().wallObject.SetActive(false);
        // queue.Enqueue(new Vector3(maxRoomSize, 0, halfRoomSize));
        // directionQueue.Enqueue(Vector3.left);
        
        while (queue.Count != 0)
        {
            Vector3 currentCase = (Vector3) queue.Dequeue();
            Vector3 currentDirection = (Vector3) directionQueue.Dequeue();
            int randomNumber = Random.Range(1,4); // between 1 and 3
            switch (randomNumber)
            {
                case 1 : // going forward
                    TryToPathForward(currentCase, currentDirection, maxRoomSize, true);
                    break;
                case 2 : // going to a side
                    TryToPathToSide(currentCase, currentDirection, maxRoomSize, true);
                    break;
                case 3 : // duplicating path
                    TryToDuplicatePath(currentCase, currentDirection, maxRoomSize);
                    break;
            }
            yield return new WaitForSeconds(6);
        }
        yield return null;
        // TODO : if can't turn, try to go in front but like as a last resort (other wise it won't stop go between front and side in a loop)
        // TODO : when go forward, check if the sides and front and upperside of the forward are not allready path, meaning that it shouldn't go there (only if the 3 are you stop)
        // TODO : check if the new path isn't allready a path, if it is, stop because it means we have reached an other path
        // TODO : check if there isn't some multiples walls that are still packed, otherwise, add some more path
        // TODO : YOU REMOVED THE ENNEMY & YOU LEFT ONLY THE UPPER PATH ON THE FIRST ROOM
    }

    /// <param name="isANativePath"> should be true if this was meant to just be a forward path, and false if it was called from a TryPathToSide function (to avoid creating a loop)</param>
    private void TryToPathForward(Vector3 currentCase, Vector3 currentDirection, int maxRoomSize, bool isANativePath)
    {
        Debug.Log("<color=blue>TryToPathForward</color>");
        Vector3 newCaseVector = currentCase + currentDirection;
        // Debug.Log("<color=red>currentCase = [" + currentCase.x + "," + currentCase.z + "]</color> + <color=purple>currentDirection = [" + currentDirection.x + "," + currentDirection.z + "]</color> = <color=green>newCaseVector = [" + newCaseVector.x + "," + newCaseVector.z + "]</color>");
        if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if new case is within the room
        {
            casesArray[(int)newCaseVector.x, (int)newCaseVector.z].GetComponent<Case>().wallObject.SetActive(false); // set to path
            queue.Enqueue(newCaseVector); // queue new case
            directionQueue.Enqueue(currentDirection); // queue same direction
        }
        else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0) // if new case is outside the room
        {
            Debug.Log("<color=blue>could not go forward so going on side</color>");
            TryToPathToSide(currentCase, currentDirection, maxRoomSize, true); // try to path to the side
        }
    }


    /// <param name="isNewDirectionRandom"> true if the new direction is a perpendicular random, false if one side was tried and we need to try the other one</param>
    private void TryToPathToSide(Vector3 currentCase, Vector3 currentDirection, int maxRoomSize, bool isNewDirectionRandom)
    {
        Debug.Log("<color=green>TryToPathToSide</color>");

        if(isNewDirectionRandom) // if the new direction should be random
        {
            if(currentDirection == Vector3.back || currentDirection == Vector3.forward) // if direction was forward or back
                currentDirection = (Random.Range(0,2) == 0) ? Vector3.right : Vector3.left; // set it to left or right
            else // if direction was left or right
                currentDirection = (Random.Range(0,2) == 0) ? Vector3.forward : Vector3.back; // set it to forward or back
        }
        else // if we tried one direction and it wasn't good (never really liked harry styles anyway...)
        {
            currentDirection = -currentDirection; // return vector inverse (the other side) (big brain moove i know 🧠)
        }

        Vector3 newCaseVector = currentCase + currentDirection;
        //Debug.Log("<color=red>currentCase = [" + currentCase.x + "," + currentCase.z + "]</color> + <color=purple>currentDirection = [" + currentDirection.x + "," + currentDirection.z + "]</color> = <color=green>newCaseVector = [" + newCaseVector.x + "," + newCaseVector.z + "]</color>");
        if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if new case is within the room
        {
            bool isAboveCaseAPath = true; // they might be outside the bound of the array
            bool isBelowCaseAPath = true; // they start at true, so if they are not set, it count as a no
            if(currentDirection == Vector3.left || currentDirection == Vector3.right) // if turning left or right, the "above" and "below" are up and down
            {
                if((int)newCaseVector.z + 1 <= maxRoomSize && (int)newCaseVector.z - 1 >= 0) // if above and below are within the room
                {
                    //Debug.Log("z = " + (int)newCaseVector.z + "z + 1 = " + ((int)newCaseVector.z + 1) + " z - 1 = " + ((int)newCaseVector.z - 1));
                    isAboveCaseAPath = !casesArray[(int)newCaseVector.x, (int)newCaseVector.z + 1].GetComponent<Case>().wallObject.activeInHierarchy;
                    isBelowCaseAPath = !casesArray[(int)newCaseVector.x, (int)newCaseVector.z - 1].GetComponent<Case>().wallObject.activeInHierarchy;
                }
            }
            else // if turning to forward or back, the "above" and "below" are on the sides
            {
                if((int)newCaseVector.x + 1 <= maxRoomSize && (int)newCaseVector.x - 1 >= 0) // if above and below are within the room
                {
                    //Debug.Log("x = " + (int)newCaseVector.x + "x + 1 = " + ((int)newCaseVector.x + 1) + " x - 1 = " + ((int)newCaseVector.x - 1));
                    isAboveCaseAPath = !casesArray[(int)newCaseVector.x + 1, (int)newCaseVector.z].GetComponent<Case>().wallObject.activeInHierarchy;
                    isBelowCaseAPath = !casesArray[(int)newCaseVector.x - 1, (int)newCaseVector.z].GetComponent<Case>().wallObject.activeInHierarchy;
                }
            }

            if(isAboveCaseAPath == false && isBelowCaseAPath == false) // check if case "above" and "below" the new case are not allready path
            {
                casesArray[(int)newCaseVector.x, (int)newCaseVector.z].GetComponent<Case>().wallObject.SetActive(false); // set to path
                queue.Enqueue(newCaseVector); // queue new case
                directionQueue.Enqueue(currentDirection); // queue new direction
            }
            else // if they are allready path it means that there is no walls between 2 path and we shouldn't make that case a path
            {
                if(isNewDirectionRandom) // if first time trying
                {
                    Debug.Log("<color=green>could not go to a side so going on the other one</color>");
                    TryToPathToSide(currentCase, currentDirection, maxRoomSize, false); // try with the other direction
                } // else it means that we allready tried and it still doesn't works (meaning both sides are no no) so i guess we stop
            }
        }
        else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0) // if new case is outside the room
        {
            if(isNewDirectionRandom) // if first time trying
            {
                Debug.Log("<color=green>could not go to a side so going on the other one</color>");
                TryToPathToSide(currentCase, currentDirection, maxRoomSize, false); // try with the other direction
            } // else it means that we allready tried and it still doesn't works (meaning both sides are no no) so i guess we stop
        }
    }

    private void TryToDuplicatePath(Vector3 currentCase, Vector3 currentDirection, int maxRoomSize)
    {
        Debug.Log("<color=purple>TryToDuplicatePath</color>");
        TryToPathForward(currentCase, currentDirection, maxRoomSize, true);
        TryToPathToSide(currentCase, currentDirection, maxRoomSize, true);
    }
}
