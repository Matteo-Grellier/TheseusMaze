using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Tooltip("Mandatory !")]
    [SerializeField] private GameObject roomPrefab;
    [Tooltip("check if the maze should be randomly generated, and uncheck if it must use data from a JSON object")]
    /*[SerializeField]*/ private bool isRandomlyGenerated;
    private int mazeToGenerateID = 0;

    [Header("Generation Settings")]
    [Tooltip("Number of rooms in a side of the maze, if mazeSize = 3, there will be 9 rooms")]
    [Range(2, 50)] [SerializeField] private int mazeSize;
    [Tooltip("Number of cases in a side of the room, if roomSize = 6, there will be 36 rooms")]
    [Range(7, 50)] [SerializeField] private int roomSize;

    private int numberOfRooms;
    private int iteration = 0;
    private int rowPosition = 0;
    private int columnPosition = 0;
    private bool isDataFetched = false;
    public bool isDoneGenerating = false;
    private bool isEndRoomSet = false;
    private bool isKeyRoomSet = false;
    private int roomsStillGenerating; //rooms that are still being generated, when 0, everything is done
    public int RoomsStillGenerating
    {
        get { return roomsStillGenerating;}
        set
        {
            roomsStillGenerating = value;
        }
    }
    private Room[] mazeRoomsArray;
    public string[ , ] mazeArray;
    public MazeObject maze = new MazeObject();

    #region JSON Serialization classes

    [Tooltip("just for debug")]
    /*[SerializeField]*/ private TextAsset jsonText;

    [Tooltip("just for debug")]
    /*[SerializeField]*/ private bool useDebugJSON;


    [System.Serializable]
    public class CaseObject
    {
        public int cellid;
        public int roomid;
        public string state;
    }

    [System.Serializable]
    public class RoomObject
    {
        public int roomid;
        public int mazeid;
        public bool leftDoor;
        public bool upDoor;
        public bool rightDoor;
        public bool downDoor;
        public bool isEndRoom;
        public List<CaseObject> cases;
    }

    [System.Serializable]
    public class MazeObject
    {
        public int mazeid;
        public string mazeName;
        public List<RoomObject> rooms;
    }

    [System.Serializable]
    public class MazeList
    {
        public List<MazeObject> mazes;
    }

    #endregion

    public void SetGenerationInformations(bool _isRandomelyGenerated, int _mazeToGenerateID)
    {
        isRandomlyGenerated = _isRandomelyGenerated;
        mazeToGenerateID = _mazeToGenerateID;
    }

    public void StartMazeGeneration()
    {
        if (!isRandomlyGenerated)
        {
            Debug.Log("[bug] randomely generated");
            if (!useDebugJSON)
                StartCoroutine(APIManager.GetOneMazeFromAPI(mazeToGenerateID, this)); // get the map from the API utils
            else
                SetMazeValues(""); // shortcut when using DebugJson
        }
        else
        {
            Debug.Log("[bug] not randomely generated");
            isDataFetched = true; // if is randomely generated, there is no data to fetch
            numberOfRooms = mazeSize * mazeSize;
            roomsStillGenerating = numberOfRooms;
            mazeArray = new string[roomSize * (int)Mathf.Sqrt(numberOfRooms), roomSize * (int)Mathf.Sqrt(numberOfRooms)]; // Sqrt because we need the number of room on one size
            mazeRoomsArray = new Room[numberOfRooms];
            Debug.Log("[bug] not randomely generated");
        }
    }

    void Update()
    {
        if (isDataFetched && !isDoneGenerating)
        {
            if (iteration < numberOfRooms)
            {
                GameObject room = Instantiate(roomPrefab, new Vector3((roomSize/2) + (0 + roomSize) * rowPosition, 0, (roomSize/2) +  (0 + roomSize) * columnPosition), Quaternion.Euler(new Vector3(0, 0, 0)));
                room.transform.parent = gameObject.transform; //set the room as child of the maze object
                Room newlyCreatedRoomScript = room.GetComponent<Room>();
                newlyCreatedRoomScript.mazeReference = this;
                newlyCreatedRoomScript.roomSize = roomSize;
                newlyCreatedRoomScript.roomArray = new string [roomSize, roomSize];
                if(!isRandomlyGenerated) // fill room if not randomely generated and not edit mode
                {
                    newlyCreatedRoomScript.room = maze.rooms[iteration];
                }
                else if (GameManager.instance.isEditMode) // if is generated but in edit mode
                {
                    RoomObject newRoomObject = new RoomObject();
                    newRoomObject.roomid = iteration;
                    newlyCreatedRoomScript.room = newRoomObject;
                    newRoomObject.cases = new List<CaseObject>();
                    maze.rooms.Add(newRoomObject);
                }
                else // is generated random
                {
                    newlyCreatedRoomScript.casesArray = new GameObject [roomSize, roomSize];
                    if (!isEndRoomSet)
                    {
                        int randomNumber = Random.Range(0,8);
                        if(randomNumber == 3 || iteration == numberOfRooms - 1) // if last
                        {
                            newlyCreatedRoomScript.room.isEndRoom = true;
                            isEndRoomSet = true;
                        }
                    }
                    if(!isKeyRoomSet)
                    {
                        int randomNumber = Random.Range(0,5);
                        if(randomNumber == 1 || iteration == numberOfRooms - 1) // if last
                        {
                            Debug.Log("<color=yellow>[key] Key Room Set : " + iteration + " </color>");
                            newlyCreatedRoomScript.isAKeyRoom = true;
                            isKeyRoomSet = true;
                        }
                    }
                }
                newlyCreatedRoomScript.roomID = iteration;
                room.transform.localScale = new Vector3(roomSize, 1, roomSize);
                mazeRoomsArray[iteration] = newlyCreatedRoomScript;
                iteration++;
                columnPosition = iteration % mazeSize;
                if (columnPosition == 0 )
                    rowPosition++;
            }
            else
            {
                if(roomsStillGenerating == 0)
                {
                    Debug.Log("<color=red>start maze array generation</color>");
                    StartCoroutine(CreateMazeArray());
                }
            }
        }
    }

    public void SetMazeValues(string _jsonText)
    {
        maze = (useDebugJSON) ? JsonUtility.FromJson<MazeObject>(jsonText.text) : JsonUtility.FromJson<MazeObject>(_jsonText);

        mazeSize = (int)Mathf.Round(Mathf.Sqrt(maze.rooms.Count)); // mazeSize is a sqrt of maze.rooms beacause it is supposed to be the number of rooms on one side
        roomSize = (int)Mathf.Round(Mathf.Sqrt(maze.rooms[0].cases.Count)); // same here
        numberOfRooms = maze.rooms.Count;
        roomsStillGenerating = numberOfRooms;
        mazeArray = new string[roomSize * mazeSize, roomSize * mazeSize];
        mazeRoomsArray = new Room[numberOfRooms];
        isDataFetched = true; // let the generation begin
    }

    int x = 0;
    int innerX = 0;
    int innerZ = 0;
    int z = 0;
    int room = 0;
    private IEnumerator CreateMazeArray()
    {
        if (z < mazeSize * roomSize) // if Z is full done then it's end of mazeGeneration
        {
            if (x < mazeSize * roomSize) // if X is bigger than maze cases size then change the Z
            {
                if (innerX >= roomSize)
                {
                    innerX = 0;
                    if ( room + mazeSize < mazeSize * mazeSize)
                    {
                        room += mazeSize; // because they are stored in y then x
                    }
                    else
                    {
                        Debug.LogError("This part of the function should never be called !");
                        if (innerZ == (roomSize - 1)) // if at the last line of the room row
                            room = room - ((mazeSize * 3) - 1); // go to the first room of the next row
                        else
                            room = room - (mazeSize * 3); // go to the first room of that row
                    }
                }
                // Debug.Log("[maze] mazeArray[" + x + "," + z +"] = mazeRoomsArray[" + room + "]roomArray[" + innerX + "," + innerZ + "] = " + mazeRoomsArray[room].roomArray[innerX,innerZ]);
                // if (mazeRoomsArray[room].roomArray[innerX,innerZ] == null)
                //     Debug.Log("<color=red>mazeRoomsArray["+ room + "].roomArray["+ innerX+","+innerZ+"] == null</color>");
                // else
                //     Debug.Log("<color=green>not nul it's okay : " + mazeRoomsArray[room].roomArray[innerX,innerZ] + "</color>");

                mazeArray[x,z] = mazeRoomsArray[room].roomArray[innerX,innerZ];
                x++;
                innerX++;
            }
            else
            {
                z++;
                x = 0;
                if ( room + mazeSize < mazeSize * mazeSize)
                {
                    room += mazeSize; // because they are stored in y then x
                }
                else
                {
                    // Debug.Log("[here] innerY+1 : " + (innerY+1) + " = " + roomSize + " ? && innerX+1 : " + (innerX+1) + " = " + roomSize + " ?");
                    if ((innerZ + 1) == roomSize && (innerX + 1) >= roomSize) // if at the last line of the room row
                    {
                        // Debug.Log("retour nouvelle ligne");
                        room = room - ((mazeSize * 3) - 1); // go to the first room of the next row
                        innerZ = 0;
                    }
                    else
                    {
                        room = room - (mazeSize * 3); // go to the first room of that row
                        innerZ++;
                    }
                }
                innerX = 0;
            }
        }
        else
        {
            Debug.Log("<color=red>[maze] Done Generating </color>");
            // Print2DStringArray("EndMaze", mazeArray);
            isDoneGenerating = true;
            yield return null;
        }
    }

    public static void Print2DStringArray(string beginingMessage, string[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Debug.Log("2DArray : "+ beginingMessage +" [" + i + "," + j + "] : " + matrix[i,j]);
            }
        }
    }
}
