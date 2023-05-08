using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMapBtn : MonoBehaviour
{
    [SerializeField] private GameObject saveBtn;

    private void Start() 
    {
        if (GameManager.instance.isEditMode)
            saveBtn.SetActive(true);
    }

    public void BtnSaveMap() 
    {
        if (GameManager.instance.isEditingNewlyCreatedMap)
            GameManager.instance.SaveNewMap();
        else
            GameManager.instance.UpdateMap();
    }
}
