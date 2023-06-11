using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{

    [SerializeField] private TMP_Text gameOverStateText;

    private void Update()
    {
        gameOverStateText.text = GameManager.instance.isWin ? "WIN" : "LOSE";   
        // gameOverStateText.color = GameManager.instance.isWin ? "" : "#FF4545";
        gameOverStateText.color = GameManager.instance.isWin ? new Color32(195, 139, 197, 255) : new Color32(255, 69, 69, 255);
    }

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
