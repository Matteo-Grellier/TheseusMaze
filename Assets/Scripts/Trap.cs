using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private AudioSource trapSound;
    [SerializeField] private AudioSource aieSound;
    private Player player;
    private float trapTimer;

    private void Awake() 
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Debug.LogWarning("ONTRIGGERENTER" + other.gameObject.name + " " + GameManager.instance.enemy.name);

        if(other.gameObject == player.gameObject)
        {
            player.speed = 0f;
            player.animator.SetBool("Run_Anim", false);
            player.isTrapped = true;
            StartCoroutine(Countdown(other, 3f));
            trapSound.Play();
            aieSound.Play();
        } 
        else if (!other.isTrigger && other.gameObject == GameManager.instance.enemy)
        {
            trapSound.Play();
            Debug.LogWarning("WTF LES AMIS LES AMIS LES A MIS");
            GameManager.instance.enemy.GetComponent<Enemy>().speed = 0f;
            StartCoroutine(Countdown(other, 1.5f));
        }
    }

    private IEnumerator Countdown(Collider other, float time) 
    {
            yield return new WaitForSeconds(time); //wait trapTimer seconds

            if(other.gameObject == player.gameObject)
            {
                player.animator.SetBool("Run_Anim", true);
                player.isTrapped = false;
                player.speed = 5f;

            } else if (other.gameObject == GameManager.instance.enemy)
            {
                GameManager.instance.enemy.GetComponent<Enemy>().speed = 2.5f;
                GameManager.instance.enemy.GetComponent<Enemy>().isMoving = true;
            }
    }
}
