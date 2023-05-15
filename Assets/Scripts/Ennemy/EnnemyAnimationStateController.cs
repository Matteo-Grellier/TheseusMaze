using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAnimationStateController : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Ennemy currentEnemy;
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
