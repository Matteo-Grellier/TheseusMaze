using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        // Debug.LogWarning("ONTRIGGERENTER" + other.gameObject.name + " " + GameManager.instance.enemy.name);

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().speed = 1f;
			other.gameObject.GetComponent<Player>().animator.speed = 0.5f;
        } 
        else if (!other.isTrigger && other.gameObject == GameManager.instance.enemy)
        {
            GameManager.instance.enemy.GetComponent<Enemy>().speed = 1f;
            GameManager.instance.enemy.GetComponent<Enemy>().enemyAnimationController.AnimatorSpeed = 0.5f;
        }
    }

    private void OnTriggerExit(Collider other) {

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().speed = 5f;
			other.gameObject.GetComponent<Player>().animator.speed = 1f;
        }
        else if (other.isTrigger && other.gameObject == GameManager.instance.enemy)
        {
            GameManager.instance.enemy.GetComponent<Enemy>().speed = 2.5f;
            GameManager.instance.enemy.GetComponent<Enemy>().enemyAnimationController.AnimatorSpeed = 1f;
        }
    }
}
