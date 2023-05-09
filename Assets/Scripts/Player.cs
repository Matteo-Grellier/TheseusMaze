using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool hasTrap = false;

    public bool hasKey = false;

    public float speed = 5f;

    public Vector3 move;

    public Camera cam;

    Vector3 velocity;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");

        float z = Input.GetAxis("Vertical");

        move = cam.transform.right * x + cam.transform.forward * z;

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
