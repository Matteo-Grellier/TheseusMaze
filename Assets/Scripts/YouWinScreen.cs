using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouWinScreen : MonoBehaviour
{
    [SerializeField] private GameObject fond;
    [SerializeField] private GameObject txt;

    void Awake()
    {
        // GameManager.instance.youWinScreen = this;
    }

    public void WinMenuActivate()
    {
        fond.SetActive(true);
        txt.SetActive(true);
    }
}
