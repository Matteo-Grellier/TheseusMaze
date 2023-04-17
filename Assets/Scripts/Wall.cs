using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Tooltip("reference to the parent case")] [SerializeField] private Case parentcase;

    // when intercept a click event, transmit it to the case
    private void OnMouseDown() 
    {
        parentcase.CaseClicked();
    }
}
