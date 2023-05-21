using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
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
