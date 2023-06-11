using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationStateController : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Enemy currentEnemy;

    public float AnimatorSpeed
    {
        get
        {
            return animator.speed;
        }
        set
        {
            animator.speed = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isGameOver) {
            animator.SetBool("isCatchingPlayer", true);
            return;
        }

        if(currentEnemy.speed > 0)
            animator.SetBool("isMoving", currentEnemy.isMoving);
        else 
            animator.SetBool("isMoving", false);
    }
}
