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
    public bool isEditingNewlyCreatedMap = false;
    public bool isRandomlyGenerated = true;
    public int mapToGenerateId = 0;

    private GameObject saveMapBtn;
    public Maze mazeReference = null; // will auto get the reference of the ONLY Maze on the scene
    private bool asLaunchedGeneration = false;

    public bool isMenu = false;

    public bool isGameOver = false;
    public bool isWin = false;

    [SerializeField]
    private GameObject enemyPrefab;
    private GameObject enemy;

    public Player player;

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

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update() 
    {
        string nameOfActiveScene = SceneManager.GetActiveScene().name;

        if(nameOfActiveScene == "GameOverScene") return;

        if(player == null && nameOfActiveScene != "Menu" && nameOfActiveScene != "EditScene")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        if(enemy == null && mazeReference && mazeReference.isDoneGenerating && nameOfActiveScene != "EditScene")
        {
            enemy = Instantiate(enemyPrefab);
            Debug.Log("I CREATE THE enemy" + enemy);
        }

        if(isGameOver) HandleGameOver();

        if (!asLaunchedGeneration && nameOfActiveScene == "GameScene" && mazeReference == null)
        {
            mazeReference = GameObject.Find("Maze").GetComponent<Maze>();
            mazeReference.SetGenerationInformations(isRandomlyGenerated, mapToGenerateId);
            mazeReference.StartMazeGeneration();
            asLaunchedGeneration = true;
        }
        else if (!asLaunchedGeneration && nameOfActiveScene == "EditScene")
        {
            mazeReference = GameObject.Find("Maze").GetComponent<Maze>();
            mazeReference.SetGenerationInformations(isRandomlyGenerated, mapToGenerateId);
            mazeReference.StartMazeGeneration();
            asLaunchedGeneration = true;
        }
        else if (!asLaunchedGeneration && nameOfActiveScene == "Mathéo")
        {
            mazeReference = GameObject.Find("Maze").GetComponent<Maze>();
            mazeReference.SetGenerationInformations(true, 0);
            mazeReference.StartMazeGeneration();
            asLaunchedGeneration = true;
        }
    }

    // called when changing the active scene
    private void OnActiveSceneChange(Scene current, Scene next)
    {
        Debug.LogWarning("switching to " + next.name + " Scene");

        string nameOfActiveScene = SceneManager.GetActiveScene().name;

        if(!isMenu && (nameOfActiveScene == "Menu" || nameOfActiveScene == "GameOverScene"))
             isMenu = true;
        else if(isMenu && nameOfActiveScene != "Menu" && nameOfActiveScene != "GameOverScene")
            isMenu = false;
    }

    private void OnSceneLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.LogWarning("Load scene" + scene.name);

        string nameOfActiveScene = SceneManager.GetActiveScene().name;

        if((nameOfActiveScene == "Menu" || nameOfActiveScene == "GameOverScene"))
             isMenu = true;
        else if(nameOfActiveScene != "Menu" && nameOfActiveScene != "GameOverScene")
            isMenu = false;
    }

    public void SaveNewMap(string mazeName)
    {
        mazeReference.maze.mazeName = mazeName;
        StartCoroutine(APIManager.PostMazeToAPI(mazeReference.maze));
    }

    public void UpdateMap(string mazeName)
    {
        mazeReference.maze.mazeName = mazeName;
        StartCoroutine(APIManager.UpdateMazeInAPI(mazeReference.maze.mazeid, mazeReference.maze));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.sceneLoaded -= OnSceneLoading;
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoading;
    }

    private void HandleGameOver()
    {
        player.gameObject.SetActive(false);
        player.GetComponentInChildren<Camera>().enabled = false;
        enemy.transform.GetChild(0).gameObject.SetActive(true);
        enemy.GetComponentInChildren<Camera>().enabled = true;
        
        StartCoroutine(WaitAndLoadGameOverScene());
    }

    private IEnumerator WaitAndLoadGameOverScene()
    {
        yield return new WaitForSeconds(3);
        LoadScene("GameOverScene");
    }

    public void Win()
    {
        isGameOver = true;
        isWin = true;
        LoadScene("GameOverScene");
    }
}
