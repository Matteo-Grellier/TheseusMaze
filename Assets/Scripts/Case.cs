using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    [Header("Mandatory References")]

    public GameObject wallObject;
    public GameObject debugCase;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }

    void OnMouseDown()
    {
        CaseClicked();
    }

    public void CaseClicked()
    {
        if (gameManager.isEditMode == true)
        {
            wallObject.SetActive(!wallObject.gameObject.activeInHierarchy);
        }
    }
}
