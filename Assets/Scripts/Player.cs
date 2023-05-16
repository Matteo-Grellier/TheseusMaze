using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool hasTrap = false;
    public bool hasKey = false;

    public float speed = 2f;

    private GameObject previousCase;
    private GameObject currentCase;

    private RaycastHit hit;
    private bool raycast;

    public Vector3 move;

    public GameObject Trap;

    public Camera cam;

    Vector3 velocity;

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");

        float z = Input.GetAxis("Vertical");

        velocity = new Vector3(x, 0, z);

        move = cam.transform.right * x + transform.forward * z;

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }

    private void Update()
    {
        raycast = Physics.Raycast(transform.position, transform.forward, out hit, 2f);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            takeTrap();
        }

        showPreviewPlacement();
    }

    void takeTrap()
    {

        if (raycast)
        {
            GameObject trap = hit.collider.gameObject.GetComponent<Case>().trapObject;
            GameObject trapPreview = hit.collider.gameObject.GetComponent<Case>().previewObject;
            // Si il y a un trap et que le joueur n'en a pas
            if (trap.activeSelf && hasTrap == false)
            {
                hasTrap = true;
                trap.SetActive(false);
            }
            // Si pas de trap et que le joueur en a un
            else if (!trap.activeSelf && hasTrap == true)
            {
                // Si il n'y a pas de mur, de boue ou de gravier
                if (!hit.collider.gameObject.GetComponent<Case>().wallObject.activeSelf && !hit.collider.gameObject.GetComponent<Case>().mudObject.activeSelf && !hit.collider.gameObject.GetComponent<Case>().gravelObject.activeSelf)
                {
                    trap.SetActive(true);
                    trapPreview.SetActive(false);
                    hasTrap = false;
                }
            }
        }
    }

    void showPreviewPlacement()
    {
        if (hasTrap == true)
        {
            currentCase = hit.collider.gameObject.GetComponent<Case>().gameObject;
            if (currentCase != previousCase)
            {
                if (previousCase != null)
                {
                    previousCase.GetComponent<Case>().previewObject.SetActive(false);
                }
                if (currentCase != null)
                {

                    if (currentCase.GetComponent<Case>().wallObject.activeSelf || currentCase.GetComponent<Case>().mudObject.activeSelf || currentCase.GetComponent<Case>().gravelObject.activeSelf)
                    {
                        // change preview color to red
                        currentCase.GetComponent<Case>().previewObject.GetComponent<Renderer>().material.color = Color.red;
                    }
                    if (currentCase.GetComponent<Case>().trapObject.activeSelf)
                    {
                        currentCase.GetComponent<Case>().previewObject.SetActive(false);
                    }else
                    {
                        currentCase.GetComponent<Case>().previewObject.SetActive(true);
                    }
                }
                previousCase = currentCase;
            }
        }
    }
}
