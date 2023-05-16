using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool hasTrap = false;

	public bool isTrapped = false;
    public bool isWalkingOnGravel = false;

    public bool hasKey = false;

    public float speed = 2f;



    private GameObject previousCase;
    private GameObject currentCase;

	private GameObject currentFocusTrap;
	private GameObject previousFocusTrap;

	public Animator animator;

    private RaycastHit hit;
    private RaycastHit hit2;
    private bool raycast;

    public Vector3 move;

    public GameObject Trap;

    public Camera cam;

    Vector3 velocity;

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");

        float z = Input.GetAxis("Vertical");

		if (x != 0 && !isTrapped || z != 0 && !isTrapped)
		{
			animator.SetBool("Run_Anim", true);
		}
		else
		{
			animator.SetBool("Run_Anim", false);
		}

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
		focusTrap();
        showPreviewPlacement();
    }

    void takeTrap()
    {
        if (raycast && hit.collider.gameObject.name == "Case(Clone)") // when click on a case (hopefully with a trap)
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

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit2, 2f, LayerMask.GetMask("btn"))) // when click on elevator btn
        {
            Debug.Log("<color=purple>" + hit2.collider.gameObject.name + "</color>");
            EndButton elevatorScript = hit2.collider.gameObject.GetComponent<EndButton>();
            Debug.Log("<color=green> isPlayerIn = true </color>");
            elevatorScript.OnClickedByPlayer();
        }
    }

	void focusTrap()
	{
		if (raycast)
		{
			currentFocusTrap = hit.collider.gameObject.GetComponent<Case>().trapObject;
			if (currentFocusTrap != previousFocusTrap)
			{
				if (previousFocusTrap != null)
				{
					previousFocusTrap.GetComponent<Renderer>().material.color = Color.white;
				}
				if (currentFocusTrap != null)
				{
					currentFocusTrap.GetComponent<Renderer>().material.color = Color.yellow;
				}
				previousFocusTrap = currentFocusTrap;
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
