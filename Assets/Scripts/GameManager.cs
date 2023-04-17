using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // static self reference to let anyone fetch its reference without using GameObject.Find() (saves computing and error)
    public static GameManager instance;

    [Tooltip("If checked, will let the player click on the cases to edit them, must be set in editor")]
    public bool isEditMode;

    void Start()
    {
        // if null, set itself as the instance of the GameManager class
        if (instance == null)
        {
            instance = this;
        }
    }
}
