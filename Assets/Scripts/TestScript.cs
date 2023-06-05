using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentDirection = Vector3.forward;
        currentDirection = new Vector3(currentDirection.z, 0, currentDirection.x);
        Debug.Log("we used Vector3.forward = " + Vector3.forward);
        Debug.Log("Vector3.right = " + Vector3.right + " currentDirection = " + currentDirection);
    }
}
