using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Tooltip("Mandatory !")]
    [SerializeField] private GameObject roomPrefab;

    [Tooltip("check if the maze should be randomly generated, and uncheck if it must use data from a JSON object")]
    [SerializeField] private bool isRandomlyGenerated;

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
    private bool isDoneGenerating = false;

    #region JSON Serialization classes

    [SerializeField] private TextAsset jsonText;

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
        public string mazename;
        public List<RoomObject> rooms;
    }

    [System.Serializable]
    public class MazeList
    {
        public List<MazeObject> mazes;
    }

    #endregion

    private MazeObject maze = new MazeObject();

    private void Start() 
    {
        if (!isRandomlyGenerated) 
        {
            StartCoroutine(APIManager.GetAllMazeFromAPI(this)); // get the map from the API utils
        }
        else 
        {
            isDataFetched = true; // if isn't randomely generated, there is no data to fetch
            numberOfRooms = mazeSize * mazeSize;
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
                room.GetComponent<Room>().roomSize = roomSize;
                if(!isRandomlyGenerated)
                    room.GetComponent<Room>().room = maze.rooms[iteration];
                room.transform.localScale = new Vector3(roomSize, 1, roomSize);
                iteration++;
                columnPosition = iteration % mazeSize;
                if (columnPosition == 0 )
                    rowPosition++;
            }
            else 
            {
                isDoneGenerating = true;
            }
        }
    }

    public void SetMazeValues(string _jsonText)
    {
        MazeList mazes = JsonUtility.FromJson<MazeList>(_jsonText);
        maze = mazes.mazes[0]; //set the maze as the fetched datas
        Debug.Log(maze.mazename);
        mazeSize = (int)Mathf.Round(Mathf.Sqrt(maze.rooms.Count)); // mazeSize is a sqrt of maze.rooms beacause it is supposed to be the number of rooms on one side
        roomSize = (int)Mathf.Round(Mathf.Sqrt(maze.rooms[0].cases.Count)); // same here
        numberOfRooms = maze.rooms.Count;
        isDataFetched = true; // let the generation begin
    }

}
