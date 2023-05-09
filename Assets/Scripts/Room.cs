using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject casePrefab;
    public int roomSize;
    public int roomID = 0;

    private int caseIteration = 0;
    private int caseColumn = 0;
    private int caseRow = 0;

    private bool isTrapShown = false;
    private bool isWallShown = false;
    private bool isGravelShown = false;
    private bool isMudShown = false;


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
            // + 0.5f otherwise it will be slightly offcentered, -roomSize/2.0f to place it on the left of the cube wich is CENTERED on 0,0,0 (same for the Z),
            // divide by 2.0f to get a float (not a rounded int), (-caseRow) beacause caseRow is spositive and should be place under
            Vector3 newCasePosition = new Vector3((pos.x + 0.5f) - (roomSize/2.0f) + (1 * caseColumn), 0.55f, (pos.z - 0.5f) + (roomSize/2.0f) + (-caseRow) );
            GameObject newCase = Instantiate(casePrefab, newCasePosition , Quaternion.Euler(new Vector3(0, 0, 0)));
            newCase.transform.parent = gameObject.transform; //set the case as child of the room
            newCase.GetComponent<Case>().caseId = caseIteration;
            newCase.GetComponent<Case>().RoomId = roomID;
            newCase.GetComponent<Case>().caseMazeReference = mazeReference;
            Case newCaseScript = newCase.gameObject.GetComponent<Case>();

            if (!GameManager.instance.isEditMode && !GameManager.instance.isRandomlyGenerated)  // if not in editmode and not randomelygenerated
            {
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
            else
            {
                int randomNumber = Random.Range(0,30);
                if (randomNumber >= 0 && randomNumber <= 7)
                {
                    isTrapShown = false;
                    isWallShown = true;
                    isGravelShown = false;
                    isMudShown = false;

                }
                else if(randomNumber == 10)
                {
                    isWallShown = false;
                    isTrapShown = true;
                    isGravelShown = false;
                    isMudShown = false;

                }
                else if (randomNumber == 11)
                {
                    isWallShown = false;
                    isTrapShown = false;
                    isGravelShown = true;
                    isMudShown = false;
                }
                else if (randomNumber == 12)
                {
                    isWallShown = false;
                    isTrapShown = false;
                    isGravelShown = false;
                    isMudShown = true;
                }
                else
                {
                    isTrapShown = false;
                    isWallShown = false;
                    isGravelShown = false;
                    isMudShown = false;
                }
                // Debug.Log( "room " + roomID + " : " + "caseColumn : " + caseColumn + " caseRow : " + caseRow + " value :" + roomArray[caseColumn,caseRow]);
            }

            newCaseScript.wallObject.SetActive(isWallShown);
            newCaseScript.trapObject.SetActive(isTrapShown);
            newCaseScript.gravelObject.SetActive(isGravelShown);
            newCaseScript.mudObject.SetActive(isMudShown);
            newCaseScript.debugCase.GetComponent<MeshRenderer>().material = roomMaterial;
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
