using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject casePrefab;
    [SerializeField] private int roomSize;
    private int caseIteration = 0;
    private int caseColumn = 0;
    private int caseRow = 0;
    private Vector3 pos;

    private void Start()
    {
        pos = gameObject.transform.position;
    }

    void Update()
    {
        if (caseIteration !< roomSize * roomSize)
        {
            GameObject newCase = Instantiate(casePrefab, new Vector3( pos.x + (1 * caseColumn), 1, pos.z + caseRow), Quaternion.Euler(new Vector3(0, 0, 0)));
            bool isWallShown = (Random.Range(0, 3) == 0) ? true : false; 
            newCase.gameObject.GetComponent<Case>().wallObject.SetActive(isWallShown);
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
