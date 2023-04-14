using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonReader : MonoBehaviour
{
    public TextAsset jsonText;

    [System.Serializable]
    public class Case {
        public int caseID;
        public string stateID;
        public int roomID;
    }

    [System.Serializable]
    public class Room {
        public int roomID;
        public int mazeID;
        public bool leftDoor;
        public bool upDoor;
        public bool rightDoor;
        public bool downDoor;
        public bool isEndRoom;
        public List<Case> cases;
    }

    [System.Serializable]
    public class Maze 
    {
        public int MazeID;
        public List<Room> rooms;
    }

    public Maze maze = new Maze();

    private void Start() {
        maze = JsonUtility.FromJson<Maze>(jsonText.text);
    }
}
