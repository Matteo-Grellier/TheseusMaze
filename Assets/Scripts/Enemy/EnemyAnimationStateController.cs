using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationStateController : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Enemy currentEnemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isMoving", currentEnemy.isMoving);
    }
}
