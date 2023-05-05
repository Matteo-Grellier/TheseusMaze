using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMapBtn : MonoBehaviour
{
    [SerializeField] private GameObject saveBtn;

    private void Start() 
    {
        if ( GameManager.instance.isEditMode == true)
            saveBtn.SetActive(true);
    }

    public void BtnSaveMap() 
    {
        GameManager.instance.BtnSaveMap();
    }
}
