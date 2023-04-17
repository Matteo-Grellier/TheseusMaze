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

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request sent successfully");
            var jsonDatas = www.downloadHandler.text;
            Debug.Log(jsonDatas);
        }
        else
        {
            Debug.Log("Error sending request: " + www.error);
        }
    }
}
