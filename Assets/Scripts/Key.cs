using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().hasKey = true;
            Destroy(gameObject);
        }
    }
}
