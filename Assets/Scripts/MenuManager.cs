using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject savedMapsMenu;

    public void BtnRandomMap()
    {
        GameManager.instance.LoadScene("GameScene");
    } 
    
    public void BtnSavedMaps()
    {
        mainMenu.SetActive(false);
        savedMapsMenu.SetActive(true);
        Debug.Log("[get] BtnSavedMaps");
        StartCoroutine(APIManager.GetAllMazeFromAPI(savedMapsMenu.GetComponent<SavedMapsMenu>()));
    } 

    public void BtnEditNewMap()
    {
        GameManager.instance.isEditMode = true;
        GameManager.instance.isEditingNewlyCreatedMap = true;
        GameManager.instance.LoadScene("EditScene");
    }

    public void BtnBackToMainMenu()
    {
        savedMapsMenu.SetActive(false);
        mainMenu.SetActive(true);
    } 
}
