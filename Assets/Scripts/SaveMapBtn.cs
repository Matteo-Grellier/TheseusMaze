using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMapBtn : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private TMP_InputField nameInput;

    private void Start() 
    {
        if (GameManager.instance.isEditMode)
            buttons.SetActive(true);

        if (GameManager.instance.isEditingNewlyCreatedMap)
            nameInput.text = "NewMaze";
        else
            nameInput.text = GameManager.instance.mazeReference.maze.mazeName;
    }

    public void BtnSaveMap() 
    {
        if (GameManager.instance.isEditingNewlyCreatedMap)
            GameManager.instance.SaveNewMap(nameInput.text);
        else
            GameManager.instance.UpdateMap(nameInput.text);
    }
}
