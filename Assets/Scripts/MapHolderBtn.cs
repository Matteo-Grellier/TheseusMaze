using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapHolderBtn : MonoBehaviour
{
    [SerializeField] private TMP_Text mapBtn;

    public int mazeID = 0;
    private string mazeName;
    public string MazeName
    {
        get 
        {
            return mazeName;
        }
        set 
        {
            mazeName = value;
            mapBtn.text = mazeName;
        }
    }

    public void BtnLaunchMap()
    {
        GameManager.instance.mapToGenerateId = mazeID;
        GameManager.instance.isRandomlyGenerated = false;
        GameManager.instance.LoadScene("GameScene");
    }

    public void BtnEditMap()
    {
        GameManager.instance.mapToGenerateId = mazeID;
        GameManager.instance.isEditMode = true;
        GameManager.instance.isRandomlyGenerated = false;
        GameManager.instance.isEditingNewlyCreatedMap = false;
        GameManager.instance.LoadScene("EditScene");
    }
}
