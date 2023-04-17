using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class buttonGet : MonoBehaviour
{
    public Button button;
    private string url = "http://86.217.108.20:4000/apiGetAll";
    private void Start()
    {
        button.onClick.AddListener(SendRequest);
    }

    private void SendRequest()
    {
        StartCoroutine(GetRequest());
    }

    private IEnumerator GetRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();



        switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(www.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(www.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(www.downloadHandler.text);
                    break;
            }

        var jsonDatas = www.downloadHandler.text;
        jsonDatas = JsonUtility.ToJson(jsonDatas);

        Debug.Log(jsonDatas[0]);
    }
}
