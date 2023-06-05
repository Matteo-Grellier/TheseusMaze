using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigElevator : MonoBehaviour
{
    [SerializeField] private AudioSource intro;
    [SerializeField] private Animator animator;

    private bool open = false;

    void Start()
    {
        StartCoroutine(PlaySongThenOpen());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Open();
            StopCoroutine(PlaySongThenOpen());
        }
    }

    IEnumerator PlaySongThenOpen()
    {
        intro.Play();
        yield return new WaitWhile (()=> intro.isPlaying);
        Open();
    }

    private void Open() 
    {
        animator.Play("BigElevator",  -1, 0f);
    }

}
