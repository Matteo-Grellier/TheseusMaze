using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentDirection = Vector3.back;
        currentDirection = -currentDirection;
        Debug.Log("we used Vector3.back = " + Vector3.back);
        Debug.Log("Vector3.forward = " + Vector3.forward + " currentDirection = " + currentDirection);
    }
}
