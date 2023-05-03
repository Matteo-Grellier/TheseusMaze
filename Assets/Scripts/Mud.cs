using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Debug.Log("SUPERRRRRRRRRRRRRRRRR");
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().speed = 1f;
        }
    }

    private void OnTriggerExit(Collider other) {

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().speed = 3f;
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("JADORE LA DROITE");
    }


}
