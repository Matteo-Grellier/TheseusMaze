using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedMapsMenu : MonoBehaviour
{
    [SerializeField] private GameObject mapListReference;
    [SerializeField] private GameObject mapHolderBtnRef;
    [SerializeField] private GameObject mapListRef;

    public void FetchDatas(string jsonData)
    {
        Debug.Log("[get] fetch data");
        Maze.MazeList mazeList = JsonUtility.FromJson<Maze.MazeList>(jsonData);
        for(int i = 0; i < mazeList.mazes.Count; i++)
        {
            Debug.Log("[get] one maze");
            GameObject newMapHolder = Instantiate(mapHolderBtnRef, Vector3.zero, Quaternion.Euler(Vector3.zero), mapListRef.transform);
            newMapHolder.GetComponent<MapHolderBtn>().MazeName = mazeList.mazes[i].mazeName;
            newMapHolder.GetComponent<MapHolderBtn>().mazeID = mazeList.mazes[i].mazeid;
        }
    }
}
