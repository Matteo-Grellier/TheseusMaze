using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // static self reference to let anyone fetch its reference without using GameObject.Find() (saves computing and error)
    public static GameManager instance;

    [Tooltip("If checked, will let the player click on the cases to edit them, must be set in editor")]
    public bool isEditMode;
    public bool isRandomlyGenerated = true;
    public int mapToGenerateId = 0;

    private GameObject saveMapBtn;
    private Maze mazeReference = null; // will auto get the reference of the ONLY Maze on the scene
    private bool asLaunchedGeneration = false;

    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject); // put the GameManager int he don't destroy on load
    }

    void Start()
    {
        // if null, set itself as the instance of the GameManager class
        if (instance == null)
        {
            instance = this;
        }

        SceneManager.activeSceneChanged += OnActiveSceneChange; // subscribe to the activeSceneChanged event
    }

    private void Update() 
    {
        if (!asLaunchedGeneration && SceneManager.GetActiveScene().name == "GameScene" && mazeReference == null)
        {
            mazeReference = GameObject.Find("Maze").GetComponent<Maze>();
            mazeReference.SetGenerationInformations(isRandomlyGenerated, mapToGenerateId);
            mazeReference.StartMazeGeneration();
            asLaunchedGeneration = true;
        }
        else if (!asLaunchedGeneration && SceneManager.GetActiveScene().name == "EditScene")
        {
            saveMapBtn = GameObject.Find("SaveMapBtn");
            isEditMode = true;
            saveMapBtn?.SetActive(true);
            asLaunchedGeneration = true;
        }

        Debug.Log("ActiveScene = " + SceneManager.GetActiveScene().name);
    }

    // called when changing the active scene
    private void OnActiveSceneChange(Scene current, Scene next)
    {
        Debug.Log("switching to " + next.name + " Scene");
    }

    // activated by clicking on the save button
    public void BtnSaveMap()
    {
        StartCoroutine(APIManager.PostMazeToAPI(mazeReference.maze));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
