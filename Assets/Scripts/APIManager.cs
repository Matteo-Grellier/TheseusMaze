using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private static string APIUrl = "http://86.217.108.20:4000";

    public static IEnumerator GetAllMazeFromAPI(Maze mazeReference)
    {
        UnityWebRequest www = UnityWebRequest.Get(APIUrl + "/getAllMazes");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var jsonDatas = www.downloadHandler.text;
            Debug.Log("Request sent successfully : " + jsonDatas);
            mazeReference.SetMazeValues(jsonDatas);
        }
        else
        {
            Debug.LogError("Error sending request: " + www.error);
        }
    }

    public static IEnumerator GetOneMazeFromAPI(int mazeID, Maze mazeReference)
    {
        UnityWebRequest www = UnityWebRequest.Get(APIUrl + "/getSingleMaze/" + mazeID);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var jsonDatas = www.downloadHandler.text;
            Debug.Log("Request sent successfully : " + jsonDatas);
            mazeReference.SetMazeValues(jsonDatas);
        }
        else
        {
            Debug.LogError("Error sending request: " + www.error);
        }
    }
}
