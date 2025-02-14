using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngineInternal;

public class EnemyStateMachine : MonoBehaviour
{
    public Coroutine currentBehaviour;

    // float: Speed || bool: Fallen || Triggers: Fall, Die, Hit
    [SerializeField] private EnemyState enemyState;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Animator animator; 
    [SerializeField] private Transform target;

    

    private float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpateState(EnemyState state)
    {
        if (currentBehaviour != null)
        {
            StopCoroutine(currentBehaviour);
        }
        switch(state)
        {
            case EnemyState.Idle:
                currentBehaviour = StartCoroutine(Idle());
                break;

            case EnemyState.Chase:
                currentBehaviour = StartCoroutine(Chase());
                break;

            case EnemyState.Crawl:
                currentBehaviour = StartCoroutine(Crawl());
                break;

            case EnemyState.Dead:
                currentBehaviour = StartCoroutine(Dead());

                break;
        }
    }

    private IEnumerator Idle()
    {
        yield return null;

    }

    private IEnumerator Chase()
    {
        while (true)
        {
            agent.destination = target.position;

            yield return null;
        }
    }

    private IEnumerator Crawl()
    {
        yield return null;

    }

    private IEnumerator Dead()
    {
        yield return null;
    }

    private IEnumerator FallingAnimationTimming()
    {
        yield return null;
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
