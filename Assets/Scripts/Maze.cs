using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Tooltip("Mandatory !")]
    [SerializeField] private GameObject roomPrefab;

    [Tooltip("check if the maze should be randomly generated, and uncheck if it must use data from a JSON object")]
    [SerializeField] private bool isRandomelyGenerated;

    [Header("Generation Settings")]

    [Tooltip("Number of rooms in a side of the maze, if mazeSize = 3, there will be 9 rooms")]
    [Range(2, 50)] [SerializeField] private int mazeSize;
    [Tooltip("Number of cases in a side of the room, if roomSize = 6, there will be 36 rooms")]
    [Range(7, 50)] [SerializeField] private int roomSize;

    private int numberOfRooms;
    private int iteration = 0;
    private int rowPosition = 0;
    private int columnPosition = 0;
    private bool isDoneGenerating = false;

    #region JSON Serialization classes

    [SerializeField] private TextAsset jsonText;

    [System.Serializable]
    public class CaseObject
    {
        public int caseID;
        public string stateID;
        public int roomID;
    }

    [System.Serializable]
    public class RoomObject 
    {
        public int roomID;
        public int mazeID;
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
        public int MazeID;
        public List<RoomObject> rooms;
    }

    #endregion

    private MazeObject maze = new MazeObject();

    private void Start() 
    {
        // numberOfRooms = (isGenerated) ? mazeSize*mazeSize : 0;

        if (!isRandomelyGenerated) 
        {
            maze = JsonUtility.FromJson<MazeObject>(jsonText.text);
            mazeSize = (int)Mathf.Round(Mathf.Sqrt(maze.rooms.Count)); // mazeSize is a sqrt of maze.rooms beacause it is supposed to be the number of rooms on one side
            roomSize = (int)Mathf.Round(Mathf.Sqrt(maze.rooms[0].cases.Count)); // same here
        }
        numberOfRooms = mazeSize * mazeSize; 
    }

    void Update()
    {
        if (!isDoneGenerating)
        {
            if (iteration < numberOfRooms)
            {
                GameObject room = Instantiate(roomPrefab, new Vector3( (0 + roomSize + 2) * rowPosition, 0, (0 + roomSize + 2) * columnPosition), Quaternion.Euler(new Vector3(0, 0, 0))); // +2 to let places for the walls all around
                room.GetComponent<Room>().roomSize = roomSize;
                if(!isRandomelyGenerated)
                    room.GetComponent<Room>().room = maze.rooms[iteration];
                room.transform.localScale = new Vector3(roomSize + 2, 1, roomSize + 2); // +2 to let places for the walls all around
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
}
