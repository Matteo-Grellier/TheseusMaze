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
            if (!useDebugJSON)
                StartCoroutine(APIManager.GetOneMazeFromAPI(mazeToGenerateID, this)); // get the map from the API utils
            else
                SetMazeValues(""); // shortcut when using DebugJson
        }
        else
        {
            isDataFetched = true; // if is randomely generated, there is no data to fetch
            numberOfRooms = mazeSize * mazeSize;
            roomsStillGenerating = numberOfRooms;
            mazeArray = new string[roomSize * (int)Mathf.Sqrt(numberOfRooms), roomSize * (int)Mathf.Sqrt(numberOfRooms)]; // Sqrt because we need the number of room on one size
            mazeRoomsArray = new Room[numberOfRooms];
        }
    }

    void Update()
    {
        if (isDataFetched && !isDoneGenerating)
        {
            if (iteration < numberOfRooms)
            {
                GameObject room = Instantiate(roomPrefab, new Vector3( (0 + roomSize) * rowPosition, 0, (0 + roomSize) * columnPosition), Quaternion.Euler(new Vector3(0, 0, 0)));
                room.transform.parent = gameObject.transform; //set the room as child of the maze object
                Room newlyCreatedRoomScript = room.GetComponent<Room>();
                newlyCreatedRoomScript.mazeReference = this;
                newlyCreatedRoomScript.roomSize = roomSize;
                newlyCreatedRoomScript.roomArray = new string [roomSize, roomSize];
                if(!isRandomlyGenerated && !GameManager.instance.isEditMode) // do not fill room if not generated and not edit mode
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
                    StartCoroutine(CreateMazeArray());
                }
            }
        }
    }

    public void SetMazeValues(string _jsonText)
    {
        maze = (useDebugJSON) ? JsonUtility.FromJson<MazeObject>(jsonText.text) : JsonUtility.FromJson<MazeObject>(_jsonText);
        // if (mazeToGenerateID < mazes.mazes.Count )
        //     maze = mazes.mazes[mazeToGenerateID]; //set the maze as the fetched datas
        // else
        //     maze = mazes.mazes[0];

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
    int y = 0;
    int room = 0;
    private IEnumerator CreateMazeArray()
    {
        if (y < roomSize)
        {
            if (x < mazeSize * roomSize)
            {
                if (innerX >= roomSize)
                {
                    innerX = 0;
                    room++;
                }
                // Debug.Log("mazeArray[" + x + "," + y +"] = roomArray[" + innerX + "," + y + "]" );
                mazeArray[x,y] = mazeRoomsArray[room].roomArray[innerX,y];
                x++;
                innerX++;
            }
            else
            {
                y++;
                x = 0;
                room = 0;
                innerX = 0;
            }
            yield return null;
        }
        else
        {
            // Print2DStringArray(mazeArray);
            isDoneGenerating = true;
        }
    }

    public static void Print2DStringArray(string[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                // Debug.Log("2DArray [" + i + "," + j + "] : " + matrix[i,j]);
            }
        }
    }
}
