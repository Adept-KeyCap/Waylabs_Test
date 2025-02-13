using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private EnemyState enemyState;
    // float: Speed || bool: Fallen || Triggers: Fall, Die, Hit
    [SerializeField] private Animator animator; 
    [SerializeField] private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LegsBusted()
    {
        if (animator == null)
        {

        }
        else
        {
            animator.SetTrigger("Fall");
        }
    }
}

public enum EnemyState
{
    Idle,
    Chase,
    Crawl,
    Dead
}
