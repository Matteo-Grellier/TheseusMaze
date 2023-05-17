using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    [Header("Mandatory References")]
    public GameObject wallObject;
    public GameObject gravelObject;
    public GameObject mudObject;
    public GameObject trapObject;
    public GameObject elevatorObject;
    public GameObject keyObject;
    public GameObject debugCase;
    public GameObject previewObject;

    public int caseId;
    public int RoomId;

    public Maze caseMazeReference; // set in the Case
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }

    void OnMouseDown()
    {
        CaseClicked();
    }

    public void CaseClicked()
    { 
        if (gameManager.isEditMode == true)
        {
            wallObject.SetActive(!wallObject.gameObject.activeInHierarchy);

            if (caseMazeReference.maze.rooms[RoomId].cases[caseId].state == "wall")
                caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "path";
            else
                caseMazeReference.maze.rooms[RoomId].cases[caseId].state =  "wall";
        }
    }
}
