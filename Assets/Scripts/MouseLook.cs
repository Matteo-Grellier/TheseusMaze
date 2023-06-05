using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    private bool isEscapeMode = false;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape)) //when click escape camera stop follows
        //     isEscapeMode = !isEscapeMode;

        // if (isEscapeMode)
        // {
        //     if (Input.GetKeyDown(KeyCode.Mouse0)) // when click on screan, undo escape mode
        //         isEscapeMode = !isEscapeMode;
        //     return; // if in escape mode, don't do
        // }

        if(GameManager.instance.isMenu && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Debug.LogWarning("BONSOIR JE SUIS PAS LOCK", this);
            return;
        } 
        else if(!GameManager.instance.isMenu && Cursor.lockState == CursorLockMode.None) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Debug.LogWarning("BONSOIR JE SUIS LOCK", this);
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0f);

        playerBody.Rotate(Vector3.up * mouseX);


    }
}
