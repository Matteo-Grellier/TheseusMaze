using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenueManager : MonoBehaviour
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
    } 

    public void BtnEditNewMap()
    {
        GameManager.instance.LoadScene("EditScene");
    } 

    public void BtnBackToMainMenu()
    {
        savedMapsMenu.SetActive(false);
        mainMenu.SetActive(true);
    } 

    public void BtnLaunchMap(int mapId)
    {
        GameManager.instance.mapToGenerateId = mapId;
        GameManager.instance.isRandomlyGenerated = false;
        GameManager.instance.LoadScene("GameScene");
    } 

}
