using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMapBtn : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private TMP_InputField nameInput;
    private bool isNameSet;

    private void Start()
    {
        if (GameManager.instance.isEditMode) // if is in edit mode set it active
            buttons.SetActive(true);

        if (GameManager.instance.isEditingNewlyCreatedMap) // if editing new map the the text = NewMaze
            nameInput.text = "NewMaze";
    }

    private void Update()
    {
        // if not editing new map & name not set & mazeReference not null
        if( !GameManager.instance.isEditingNewlyCreatedMap && !isNameSet && GameManager.instance.mazeReference != null)
        {
            if(GameManager.instance.mazeReference.maze.mazeName != "") // if maze not null
            {
                Debug.Log("should be good = " + GameManager.instance.mazeReference.maze.mazeName);
                nameInput.text = GameManager.instance.mazeReference.maze.mazeName; // set name as mazeName
                isNameSet = true;
            }
        }
    }

    public void BtnSaveMap()
    {
        if (GameManager.instance.isEditingNewlyCreatedMap)
            GameManager.instance.SaveNewMap(nameInput.text);
        else
            GameManager.instance.UpdateMap(nameInput.text);
    }

	public void BtnBackToMenu()
	{
		GameManager.instance.LoadScene("Menu");
	}
}
