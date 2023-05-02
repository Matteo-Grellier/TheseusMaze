using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // static self reference to let anyone fetch its reference without using GameObject.Find() (saves computing and error)
    public static GameManager instance;

    [Tooltip("If checked, will let the player click on the cases to edit them, must be set in editor")]
    public bool isEditMode;

    private Maze mazeReference; // will auto get the reference of the ONLY Maze on the scene

    void Start()
    {
        // if null, set itself as the instance of the GameManager class
        if (instance == null)
        {
            instance = this;
        }

        mazeReference = GameObject.Find("Maze").GetComponent<Maze>();
    }

    // activated by clicking on the save button
    public void BtnSaveMap()
    {
        StartCoroutine(APIManager.PostMazeToAPI(mazeReference.maze));
    }
}
