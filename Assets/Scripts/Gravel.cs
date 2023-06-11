using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravel : MonoBehaviour
{
    [SerializeField] private AudioSource gravelSound;
    public bool isWalkingOnGravel = false;
    private Player player;

    private void Start()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if(player == null)
        {
            player = GameManager.instance.player;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != player.gameObject) return;

        gravelSound.Play();
        player.isWalkingOnGravel = true;
        //TODO: player sound;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject != player.gameObject) return;

        player.isWalkingOnGravel = false;
    }
}
