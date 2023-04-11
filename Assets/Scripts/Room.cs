using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject casePrefab;
    public int roomSize;
    private int caseIteration = 0;
    private int caseColumn = 0;
    private int caseRow = 0;
    private Vector3 pos;
    private Material roomMaterial;

    private void Awake()
    {
        pos = gameObject.transform.position;
        roomMaterial = new Material(Shader.Find("Specular"));
        roomMaterial.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void Update()
    {
        if (caseIteration !< roomSize * roomSize)
        { 
            GameObject newCase = Instantiate(casePrefab, new Vector3( pos.x - (roomSize/2) + (1 * caseColumn), 0.55f, pos.z + (roomSize/2) + caseRow), Quaternion.Euler(new Vector3(0, 0, 0)));
            bool isWallShown = (Random.Range(0, 4) == 0) ? true : false; 
            Case newCaseScript = newCase.gameObject.GetComponent<Case>();
            newCaseScript.wallObject.SetActive(isWallShown);
            newCaseScript.debugCase.GetComponent<MeshRenderer>().material = roomMaterial;
            caseIteration++;
            caseColumn++;
            if (caseIteration != 0 && caseColumn == roomSize)
            {
                caseRow--;
                caseColumn = 0; 
            }
        }
    }
}
