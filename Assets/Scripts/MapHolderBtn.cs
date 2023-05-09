using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHolderBtn : MonoBehaviour
{
    [SerializeField] private int mapID = 0;

    public void BtnLaunchMap()
    {
        GameManager.instance.mapToGenerateId = mapID;
        GameManager.instance.isRandomlyGenerated = false;
        GameManager.instance.LoadScene("GameScene");
    }

    public void BtnEditMap()
    {
        GameManager.instance.mapToGenerateId = mapID;
        GameManager.instance.isEditMode = true;
        GameManager.instance.isRandomlyGenerated = false;
        GameManager.instance.isEditingNewlyCreatedMap = false;
        GameManager.instance.LoadScene("EditScene");
    }
}
