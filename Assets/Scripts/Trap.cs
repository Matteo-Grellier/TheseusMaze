using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    private Player player;

    private float trapTimer;

    private void Awake() {
        player = GameObject.Find("Player").GetComponent<Player>();
    }


    private void OnTriggerEnter(Collider other) {
        player.speed = 0f;

        trapTimer = 3f;
        StartCoroutine(Countdown3());

    }

    private IEnumerator Countdown3() {
            Debug.Log("Starting 3 second timer");
            yield return new WaitForSeconds(3); //wait 3 seconds
            Debug.Log("3 seconds passed");
            player.speed = 5f;
    }
}
