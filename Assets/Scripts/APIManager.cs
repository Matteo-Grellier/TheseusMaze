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

    public static IEnumerator PostMazeToAPI(Maze.MazeObject mazeReference)
    {
        string jsonData = "{ \"mazes\": [" +  JsonUtility.ToJson(mazeReference) + "]}";
        Debug.Log("jsonData = " + jsonData);

        var uwr = new UnityWebRequest(APIUrl + "/createMaze", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Error sending request Post : " + uwr.error);
        }
        else
        {
            Debug.Log("Request Post sent successfully ! ");
        }
    }
}
