using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCamera : MonoBehaviour
{
    private void FixedUpdate() 
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Height");
        float z = Input.GetAxis("Vertical");

        // if(Input.GetKeyDown(KeyCode.LeftShift))
        //     y = -1f;
        // if(Input.GetKeyDown(KeyCode.Space))
        //     y = 1f;

        transform.position = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
    }
}
