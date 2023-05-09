using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    private Player player;

    private void OnTriggerEnter(Collider other) {
        player.move = Vector3.zero;
    }
}
