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

    // is set in the Maze that creates it, but only if the Maze is generating randomely, otherwise it will be an empty object
    public Maze.RoomObject room;

    private void Awake()
    {
        pos = gameObject.transform.position;
        roomMaterial = new Material(Shader.Find("Specular"));
        roomMaterial.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void Update()
    {
        if (caseIteration < roomSize * roomSize)
        { 
            GameObject newCase = Instantiate(casePrefab, new Vector3( pos.x - (roomSize/2) + (1 * caseColumn), 0.55f, pos.z + (roomSize/2) + caseRow), Quaternion.Euler(new Vector3(0, 0, 0)));
            Case newCaseScript = newCase.gameObject.GetComponent<Case>();

            bool isWallShown;
            if (room.cases.Count != 0)  // if room is not set by the Maze (meaning the maze is being randomely generated) his count will be equal to 0
            {
                switch (room.cases[caseIteration].stateID) 
                {
                    case "wall" :
                        isWallShown = true;
                        break;
                    case "path" :
                        isWallShown = false;
                        break;
                    default :
                        isWallShown = false;
                        break;
                }
            }
            else 
            {
                isWallShown = (Random.Range(0, 4) == 0) ? true : false;
            }

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
