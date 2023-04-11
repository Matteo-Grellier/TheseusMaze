using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab; 

    [Tooltip("Number of rooms in a side of the maze, if mazeSize = 3, there will be 9 rooms")]
    [Range(2, 50)] [SerializeField] private int mazeSize;

    [Tooltip("Number of cases in a side of the room, if roomSize = 6, there will be 36 rooms")]
    [Range(7, 50)] [SerializeField] private int roomSize;

    private Vector3 centerPosition = new Vector3(0, 0, 0);
    private int numberOfRooms;

    private int iteration = 0;
    private int rowPosition = 0;
    private int columnPosition = 0;

    private void Start() 
    {
        numberOfRooms = mazeSize*mazeSize;
    }

    void Update()
    {
        // if all the rooms aren't spawned
        if (iteration < numberOfRooms)
        {
            GameObject room = Instantiate(roomPrefab, new Vector3(0 + roomSize * rowPosition,0, 0 + roomSize * columnPosition), Quaternion.Euler(new Vector3(0, 0, 0)));
            room.GetComponent<Room>().roomSize = roomSize;
            room.transform.localScale = new Vector3(roomSize, 1, roomSize);
            iteration++;
            columnPosition = iteration % mazeSize;
            if (columnPosition == 0 )
                rowPosition++;
        }
    }
}
