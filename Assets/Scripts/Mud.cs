using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().speed = 1f;
			other.gameObject.GetComponent<Player>().animator.speed = 0.5f;
        }
    }

    private void OnTriggerExit(Collider other) {

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().speed = 5f;
			other.gameObject.GetComponent<Player>().animator.speed = 1f;
        }
    }
}
