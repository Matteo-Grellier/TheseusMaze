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
						isTrapShown = false;
						isMudShown = false;
						isGravelShown = false;
						isKey = false;
						isElevator = false;
                        roomArray[caseColumn,caseRow] = "wall";
                        break;
                    case "path" :
						isWallShown = false;
						isTrapShown = false;
						isMudShown = false;
						isGravelShown = false;
						isKey = false;
						isElevator = false;
                        roomArray[caseColumn,caseRow] = "path";
                        break;
					case "trap" :
						isWallShown = false;
						isTrapShown = true;
						isMudShown = false;
						isGravelShown = false;
						isKey = false;
						isElevator = false;
						roomArray[caseColumn,caseRow] = "trap";
						break;
					case "gravel" :
						isWallShown = false;
						isGravelShown = true;
						isTrapShown = false;
						isMudShown = false;
						isKey = false;
						isElevator = false;
						roomArray[caseColumn,caseRow] = "gravel";
						break;
					case "mud" :
						isWallShown = false;
						isMudShown = true;
						isGravelShown = false;
						isTrapShown = false;
						isKey = false;
						isElevator = false;
						roomArray[caseColumn,caseRow] = "mud";
						break;
					case "key" :
						isWallShown = false;
						isMudShown = true;
						isGravelShown = false;
						isTrapShown = false;
						isKey = true;
						isElevator = false;
						roomArray[caseColumn,caseRow] = "key";
						break;
					case "elevator" :
						isWallShown = false;
						isMudShown = true;
						isGravelShown = false;
						isTrapShown = false;
						isKey = false;
						isElevator = true;
						roomArray[caseColumn,caseRow] = "elevator";
						break;
					default :
						isWallShown = false;
						isGravelShown = false;
						isTrapShown = false;
						isMudShown = false;
						isKey = false;
						isElevator = false;
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
                isWallShown = false;
                isTrapShown = false;
                isGravelShown = false;
                isMudShown = false;
                isTrapShown = false;
                isElevator = false;
                isKey = false;
                int randomNumber = Random.Range(0,30);

                if (isAKeyRoom)
                {
                    if (!keyIsSet && randomNumber == 1 || !keyIsSet && caseIteration == (roomSize*roomSize) - 1)// if is 1 or is not set
                    {
                        Debug.Log("<color=yellow>[key] Key Case is Set : " + caseIteration +" </color>");
                        keyIsSet = true;
                        isKey = true;
                        roomArray[caseColumn,caseRow] = "key";
                    }
                }
                if (room.isEndRoom == true)
                {
                    if (caseIteration ==  (roomSize*roomSize) / 2) // to set it in the middle of the room
                    {
                        isElevator = true;
                        roomArray[caseColumn,caseRow] = "elevator";
                    }
                }
                if (!isKey && !room.isEndRoom) // if it's a key case or a end room, no need for all this
                {
                    if (randomNumber >= 0 && randomNumber <= 7)
                    {
                        isWallShown = true;
                        roomArray[caseColumn,caseRow] = "wall";
                    }
                    else if(randomNumber == 10)
                    {
                        isTrapShown = true;
                        roomArray[caseColumn,caseRow] = "trap";
                    }
                    else if (randomNumber == 11)
                    {
                        isGravelShown = true;
                        roomArray[caseColumn,caseRow] = "gravel";
                    }
                    else if (randomNumber == 12)
                    {
                        isMudShown = true;
                        roomArray[caseColumn,caseRow] = "mud";
                    }
                    else
                    {
                        roomArray[caseColumn,caseRow] = "path";
                    }
                }
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
            }
        }
    }
}
