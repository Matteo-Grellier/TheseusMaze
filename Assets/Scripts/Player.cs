using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool hasTrap = false;

    public bool hasKey = false;

    public float speed = 2f;

    private int previousCase;
    private int currentCase;

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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            takeTrap();
        }

        showPreviewPlacement();
    }

    void takeTrap()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            Debug.Log(hit.collider.gameObject.GetComponent<Case>().trapObject.activeSelf);
            if (hit.collider.gameObject.GetComponent<Case>().trapObject.activeSelf && hasTrap == false)
            {
                hasTrap = true;
                hit.collider.gameObject.GetComponent<Case>().trapObject.SetActive(false);
            }
            else if (!hit.collider.gameObject.GetComponent<Case>().trapObject.activeSelf && hasTrap == true)
            {
                if (!hit.collider.gameObject.GetComponent<Case>().wallObject.activeSelf)
                {
                    hit.collider.gameObject.GetComponent<Case>().trapObject.SetActive(true);
                    hasTrap = false;
                }
            }
        }
    }
    void showPreviewPlacement()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, 2f);
        currentCase = hit.collider.gameObject.GetComponent<Case>().GetInstanceID();
        if (hasTrap == true)
        {
            if (previousCase == currentCase)
            {
                Debug.Log("SUPERRRRRRRRRRRRRR");
                hit.collider.gameObject.GetComponent<Case>().previewObject.SetActive(true);
                previousCase = currentCase;
            }
            else
            {
                Debug.Log("NOOOOOOOOOOOOOOOOoo");
                hit.collider.gameObject.GetComponent<Case>().previewObject.SetActive(false);
            }
        }
    }
}
