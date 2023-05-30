using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    private bool isGamePaused = false;

    [SerializeField] private GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isGamePaused)
                Resume();
            else
                Pause();
        }
    }

    private void Pause()
    {
        isGamePaused = true;
        GameManager.instance.isMenu = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        isGamePaused = false;
        GameManager.instance.isMenu = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void GoToMainMenu()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        GameManager.instance.LoadScene("Menu");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
	        UnityEditor.EditorApplication.isPlaying = false;
        #else
	        Application.Quit();
        #endif
    } 
}
