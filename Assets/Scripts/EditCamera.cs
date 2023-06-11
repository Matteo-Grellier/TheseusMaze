using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCamera : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 0.02f;

    private void FixedUpdate() 
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Height");
        float z = Input.GetAxis("Vertical");

        transform.position = new Vector3(transform.position.x + (x * cameraSpeed), transform.position.y + (y * cameraSpeed), transform.position.z + (z * cameraSpeed));
    }
}
