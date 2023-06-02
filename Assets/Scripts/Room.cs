using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private bool onlyUseUpperPath = false;
    [SerializeField] private bool useDoubleCheckOnSide = false;
    [SerializeField] private bool useStrictDoubleCheck = false;
    [SerializeField] private bool forbidCentralCase = false;
    [Range(5, 50)] [SerializeField] private int maxValueForObjectRandomRange = 5;

    [Header("Public Settings")]
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
                isElevator = false;
                isKey = false;
                isWallShown = false;

                if (room.isEndRoom) // if elevatorRoom
                {
                    if (isAKeyRoom && caseIteration == 0) // if is end room and key room, set the first case as the key
                    {
                        isKey = true;
                        roomArray[caseColumn,caseRow] = "key";
                    }
                    else if (caseIteration ==  (roomSize*roomSize) / 2) // to set the elevator in the middle of the room
                    {
                        isElevator = true;
                        roomArray[caseColumn,caseRow] = "elevator";
                    }
                    else // not the elevator so it's path
                    {
                        isWallShown = false;
                        roomArray[caseColumn,caseRow] = "path";
                    }
                }
                else // if it's not elevator room, it's full walls
                {
                    isWallShown = true;
                    roomArray[caseColumn,caseRow] = "wall";
                }

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
                //Debug.Log("room " + roomID + " : " + roomArray[0,0] + " : " + roomArray[1,0] + " : " + roomArray[0,1] + " : " + roomArray[1,1]);
                isAfterGenerationCodeExecuted = true;
                if(!room.isEndRoom) //roomID == 0)
                    StartCoroutine(RoomPathGeneration()); // room still generating set at the end of the generation
                else
                    gameObject.transform.parent.GetComponent<Maze>().RoomsStillGenerating--; // consider the room generation finished
            }
        }
    }

#region Path Generation
    
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
        if(!onlyUseUpperPath)
        {
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
        }
        
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
                    TryToPathToSide(currentCase, currentDirection, maxRoomSize, Vector3.zero);
                    break;
                case 3 : // duplicating path
                    TryToDuplicatePath(currentCase, currentDirection, maxRoomSize);
                    break;
            }
            // yield return new WaitForSeconds(6);
        }

        if(isAKeyRoom && !keyIsSet) // if is a key room and key not set
        {
            int x = 0;
            int z = 0;
            while(!keyIsSet) // while key not set, search through the cases to find a path, and set it's key to true
            {
                if(roomArray[x,z] == "path")
                {
                    casesArray[x,z].GetComponent<Case>().keyObject.SetActive(true);
                    keyIsSet = true;
                    Debug.Log("<color=yellow>[key] key set via lastcheck [" + x + "," + z + "]</color>");
                }
                else if ((x + 1) >= roomSize)
                {
                    z++;
                    x = 0;
                }
                else
                {
                    x++;
                }
            }
        }

        gameObject.transform.parent.GetComponent<Maze>().RoomsStillGenerating--; // do at the end of generation
        Debug.Log("<color=red>[room] done generating room RoomsStillGenerating=" + gameObject.transform.parent.GetComponent<Maze>().RoomsStillGenerating + "</color>", this);
        yield return null;
    }

    /// <param name="isANativePath"> should be true if this was meant to just be a forward path, and false if it was called from a TryPathToSide function (to avoid creating a loop)</param>
    private void TryToPathForward(Vector3 currentCase, Vector3 currentDirection, int maxRoomSize, bool isANativePath)
    {
        Vector3 newCaseVector = currentCase + currentDirection;
        // Debug.Log("<color=blue>TryToPathForward</color>" + newCaseVector);
        
        if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if new case is within the room
        {
            if(!IsCaseAPath(newCaseVector)) // if new case is not a path
            {
                if(IsCaseOkayToForwardTo(newCaseVector, currentDirection))
                {
                    // Debug.Log("<color=blue>path forward !</color>");
                    SetCaseToPath(newCaseVector); // set to path and maybe traps or stuffs
                    queue.Enqueue(newCaseVector); // queue new case
                    directionQueue.Enqueue(currentDirection); // queue same direction
                }
                else
                {
                    if(isANativePath) // if isn't a native path, it means that it came from a TryToPathToSide so let's not call it again or it will create A LOOP IN THE TIME SPACE CONTINIUM !!!! (wich is bad)
                    {
                        // Debug.Log("<color=blue>could not go forward so going on side</color>");
                        TryToPathToSide(currentCase, currentDirection, maxRoomSize, Vector3.zero); // try to path to the side
                    }
                }
            }
            else // else don't do anything because it's comming to a path so we stop here
            {
                // Debug.Log("<color=blue>it's a path we stoppin here</color>");
            }
        }
        else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0) // if new case is outside the room
        {
            if(isANativePath) // if isn't a native path, it means that it came from a TryToPathToSide so let's not call it again or it will create A LOOP IN THE TIME SPACE CONTINIUM !!!! (wich is bad)
            {
                // Debug.Log("<color=blue>could not go forward so going on side</color>");
                TryToPathToSide(currentCase, currentDirection, maxRoomSize, Vector3.zero); // try to path to the side
            }
        }
    }

    /// <param name="forcedNewDirection"> must be set to Vector3.zero if the new direction for the turn should be random, if one side was tried and we need to try the other one, it must be set with this new side</param>
    private void TryToPathToSide(Vector3 currentCase, Vector3 currentDirection, int maxRoomSize, Vector3 forcedNewDirection)
    {
        Vector3 newCurrentDirection;
        if(forcedNewDirection == Vector3.zero) // if the new direction should be random
        {
            if(currentDirection == Vector3.back || currentDirection == Vector3.forward) // if direction was forward or back
                newCurrentDirection = (Random.Range(0,2) == 0) ? Vector3.right : Vector3.left; // set it to left or right
            else // if direction was left or right
                newCurrentDirection = (Random.Range(0,2) == 0) ? Vector3.forward : Vector3.back; // set it to forward or back
        }
        else // if we tried one direction and it wasn't good (never really liked harry styles anyway...)
        {
            // Debug.Log("<color=green>forcedNewDirection was set = </color>" + forcedNewDirection);
            newCurrentDirection = forcedNewDirection; // return vector inverse (the other side) (big brain moove i know 🧠)
        }

        Vector3 newCaseVector = currentCase + newCurrentDirection;
        // Debug.Log("<color=green>TryToPathToSide</color>" + newCaseVector);
        if (newCaseVector.x <= maxRoomSize && newCaseVector.x >= 0 && newCaseVector.z <= maxRoomSize && newCaseVector.z >= 0) // if new case is within the room
        {
            if(!IsCaseAPath(newCaseVector)) // if new case is not a path
            {
                if(IsCaseOkayToSidePathTo(newCaseVector, newCurrentDirection) ) // check if case "above" and "below" are not path
                {
                    // Debug.Log("<color=green>path to side !</color>");
                    SetCaseToPath(newCaseVector); // set to path and maybe traps or stuffs
                    queue.Enqueue(newCaseVector); // queue new case 
                    directionQueue.Enqueue(newCurrentDirection); // queue new direction
                }
                else // if they are allready path it means that twe shouldn't make that case a path
                {
                    // Debug.Log("<color=green>can't add path here, it would make a corner</color>");
                    if(forcedNewDirection == Vector3.zero) // if first time trying
                    {
                        // Debug.Log("<color=green>could not go to a side so going on the other one</color>");
                        TryToPathToSide(currentCase, currentDirection, maxRoomSize, -newCurrentDirection); // try with the other direction -newCurrentDirection equals vector inverse
                    }
                    else
                    {
                        // Debug.Log("<color=green>could not go both side so going on a straight line from her</color>");
                        TryToPathForward(currentCase, currentDirection, maxRoomSize, false); // with the old currentDirection so the forward is not the side we tried to go to
                    }
                }
            }
            else // else don't do anything because it's comming to a path so we stop here
            {
                // Debug.Log("<color=green>it's a path so we stoppin here</color>");
            }
        }
        else if(newCaseVector.x > maxRoomSize || newCaseVector.x < 0 || newCaseVector.z > maxRoomSize || newCaseVector.z < 0) // if new case is outside the room
        {
            if(forcedNewDirection == Vector3.zero) // if first time trying
            {
                // Debug.Log("<color=green>could not go to a this side bc out of bound so going on the other one</color>");
                TryToPathToSide(currentCase, currentDirection, maxRoomSize, -newCurrentDirection); // try with the other direction -newCurrentDirection equals vector inverse
            } 
            else
            {
                // Debug.Log("<color=green>could not go both side so going on a straight line from here</color>");
                TryToPathForward(currentCase, currentDirection, maxRoomSize, false);
            }
        }
    }

    private void TryToDuplicatePath(Vector3 currentCase, Vector3 currentDirection, int maxRoomSize)
    {
        // Debug.Log("<color=purple>TryToDuplicatePath</color>");
        TryToPathForward(currentCase, currentDirection, maxRoomSize, true);
        TryToPathToSide(currentCase, currentDirection, maxRoomSize, Vector3.zero);
    } 

    private void SetCaseToPath(Vector3 currentCase)
    {
        Case caseToSetToPath = casesArray[(int)currentCase.x, (int)currentCase.z].GetComponent<Case>();
        caseToSetToPath.wallObject.SetActive(false); // wall == false first of all
        roomArray[(int)currentCase.x,(int)currentCase.z] = "path";

        int randomNumber = Random.Range(1,maxValueForObjectRandomRange + 1); // between 1 and maxValueForObjectRandomRange 
        if (isAKeyRoom) // add a key
        {
            if (!keyIsSet && randomNumber == 1 || !keyIsSet && caseIteration == (roomSize*roomSize) - 1)// if is 1 or is not set
            {
                Debug.Log("<color=yellow>[key] Key Case is Set : [" + roomID + "][" + currentCase.x + "," + currentCase.z + "] </color>");
                keyIsSet = true;
                caseToSetToPath.keyObject.SetActive(true);
                roomArray[(int)currentCase.x,(int)currentCase.z] = "key";
            }
        }
        switch(randomNumber)
        {
            case 2 : // 1 is for key
                roomArray[(int)currentCase.x,(int)currentCase.z] = "mud";
                caseToSetToPath.mudObject.SetActive(true);
                break;
            case 3 :
                roomArray[(int)currentCase.x,(int)currentCase.z] = "trap";
                caseToSetToPath.trapObject.SetActive(true);
                break;
            case 4 :
                roomArray[(int)currentCase.x,(int)currentCase.z] = "gravel";
                caseToSetToPath.gravelObject.SetActive(true);
                break;
        }
    }

    /// <summary>check if the case is a path, returns true if the case is a path false if it's not</summary>
    private bool IsCaseAPath(float caseToTestX, float caseToTestZ)
    {
        return !casesArray[(int)caseToTestX, (int)caseToTestZ].GetComponent<Case>().wallObject.activeInHierarchy;
    }
    /// <summary>check if the case is a path, returns true if the case is a path false if it's not</summary>
    private bool IsCaseAPath(Vector3 caseToTest)
    {
        return !casesArray[(int)caseToTest.x, (int)caseToTest.z].GetComponent<Case>().wallObject.activeInHierarchy;
    }

    private bool IsCaseWithinRoom(Vector3 caseToTest)
    {
        if(caseToTest.x <= (roomSize - 1) && caseToTest.x >= 0 && caseToTest.z <= (roomSize - 1) && caseToTest.z >= 0)
            return true;
        else
            return false;
    }

    /// <summary>This function checks if the cases "above" and "below" the one we want to create (depending on the direction), are not allready path meaning it would be a corner</summary>
    /// <returns>True if cases are not path (or are out of bound) and false if one of them is a path</returns>
    private bool IsCaseOkayToSidePathTo(Vector3 newCaseVector, Vector3 currentDirection)
    {
        bool? isAboveCaseAPath = null; // they might be outside the bound of the array, they start at null, so if they are not set it's different than false
        bool? isAboveAboveCaseAPath = null;
        bool? isBelowCaseAPath = null;
        bool? isBelowBelowCaseAPath = null;

        Vector3 aboveCasePosition = newCaseVector + new Vector3(currentDirection.z, 0, currentDirection.x); // transform a front in right (does a 90° clockwise)
        Vector3 aboveAboveCasePosition = newCaseVector + (new Vector3(currentDirection.z, 0, currentDirection.x) * 2);
        Vector3 belowCasePosition = newCaseVector + -(new Vector3(currentDirection.z, 0, currentDirection.x)); // the other side
        Vector3 belowBelowCasePosition = newCaseVector + -((new Vector3(currentDirection.z, 0, currentDirection.x) *2)); 
        // Debug.Log("[check] newCaseVector=" + newCaseVector + " aboveCasePosition=" + aboveCasePosition + " aboveAboveCasePosition=" + aboveAboveCasePosition + " belowCasePosition=" + belowCasePosition + " belowBelowCasePosition=" + belowBelowCasePosition);

        if(IsCaseWithinRoom(aboveCasePosition))
            isAboveCaseAPath = IsCaseAPath(aboveCasePosition);
        if(IsCaseWithinRoom(aboveAboveCasePosition))
            isAboveAboveCaseAPath = IsCaseAPath(aboveAboveCasePosition);
        if(IsCaseWithinRoom(belowCasePosition))
            isBelowCaseAPath = IsCaseAPath(belowCasePosition);
        if(IsCaseWithinRoom(belowBelowCasePosition))
            isBelowBelowCaseAPath = IsCaseAPath(belowBelowCasePosition);
        
        if (useDoubleCheckOnSide && useStrictDoubleCheck) // if strict DoubleCheck
        {
            if(isAboveCaseAPath != true && isBelowCaseAPath != true && isAboveAboveCaseAPath != true && isBelowBelowCaseAPath != true) // returns true only the case "above" AND "below" are wall or outside bound
                return true;
            else // otherwise return false, because the case is not legit to path to 
                return false;
        }
        else if (useDoubleCheckOnSide && !useStrictDoubleCheck) // if not strict DoubleCheck
        {
            if(isAboveCaseAPath != true && isBelowCaseAPath != true && (isAboveAboveCaseAPath != true || isBelowBelowCaseAPath != true)) // returns true only the case "above" AND "below" are wall or outside bound
                return true;
            else // otherwise return false, because the case is not legit to path to 
                return false;
        }
        else // if no DoubleCheck
        {
            if(isAboveCaseAPath != true && isBelowCaseAPath != true) // returns true only the case "above" AND "below" are wall or outside bound
                return true;
            else // otherwise return false, because the case is not legit to path to 
                return false;
        }
    }

    /// <summary>This function checks if you should be able to forward here, it checks the cases in front, on left, and in front of the on left</summary>
    /// <returns>True if all the cases are not paths and false if they all are paths, or if it's the central case</returns>
    private bool IsCaseOkayToForwardTo(Vector3 newCaseVector, Vector3 currentDirection)
    {
        if (forbidCentralCase && newCaseVector.x == (roomSize/ 2) && newCaseVector.z == (roomSize/ 2)) // if is central case (and the option active)
            return false; // quickly end the function, the goal is to avoind aving a central line going from up to down

        // they might be outside the bound of the array, they start at null, so if they are not set, it's different than false
        bool? isFrontCaseAPath = null;
        bool? isFirstSideCaseAPath = null; 
        bool? isFirstSideFrontCaseAPath = null;
        bool? isSecondSideCaseAPath = null;
        bool? isSecondSideFrontCaseAPath = null;

        Vector3 frontCasePosition = newCaseVector + currentDirection;
        Vector3 firstSideCasePosition = newCaseVector + new Vector3(currentDirection.z, 0, currentDirection.x); // i guess this last part should take a perpendicular to the front
        Vector3 firstSideFrontCasePosition = firstSideCasePosition + currentDirection; 
        Vector3 secondSideCasePosition = newCaseVector + -(new Vector3(currentDirection.z, 0, currentDirection.x)); // same but the other side
        Vector3 secondSideFrontCasePosition = secondSideCasePosition + currentDirection; 

        if(IsCaseWithinRoom(frontCasePosition)) // if within the room
            isFrontCaseAPath = IsCaseAPath(frontCasePosition); // check if it's a path
        if(isFrontCaseAPath != true) // if front case is outside or not a path, its okay we can return now
            return true;
        
        if(IsCaseWithinRoom(firstSideCasePosition))
            isFirstSideCaseAPath = IsCaseAPath(firstSideCasePosition);
        if(IsCaseWithinRoom(firstSideFrontCasePosition))
            isFirstSideFrontCaseAPath = IsCaseAPath(firstSideFrontCasePosition);
        if(IsCaseWithinRoom(secondSideCasePosition))
            isSecondSideCaseAPath = IsCaseAPath(secondSideCasePosition);
        if(IsCaseWithinRoom(secondSideFrontCasePosition))
            isSecondSideFrontCaseAPath = IsCaseAPath(secondSideFrontCasePosition);
        
        // returns true if the side and front side of one side a both true (because we allready checked for the front one)
        if(isFirstSideCaseAPath != true && isFirstSideFrontCaseAPath != true || isSecondSideCaseAPath != true && isSecondSideFrontCaseAPath != true)
            return true;
        else // otherwise return false, because the case is not legit to path to 
            return false;
    }

#endregion

}
