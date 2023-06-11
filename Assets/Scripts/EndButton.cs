using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndButton : MonoBehaviour
{
    public EndElevator endElevatorReference;
    private bool isClicked = false;

    public void OnClickedByPlayer()
    {
        if (!isClicked)
        {
            transform.position = new Vector3(transform.position.x + 0.02f, transform.position.y, transform.position.z);
            endElevatorReference.isPlayerIn = true;
            isClicked = true;
        }
        else
        {
            transform.position = new Vector3(transform.position.x - 0.02f, transform.position.y, transform.position.z);
            endElevatorReference.isPlayerIn = false;
            isClicked = false;
        }
    }
}
