using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngineInternal;

public class EnemyStateMachine : MonoBehaviour
{
    public Coroutine currentBehaviour;

    [Header("NavMesh AI")]
    // float: Speed || bool: Crawling || Triggers: Fall, Die, Hit
    [SerializeField] private EnemyState enemyState;
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField] private float proximityRange;
    [SerializeField] private float attackRange;
    [SerializeField, Range(1, 2)] private float modifierValue;

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool dead = false;
    private bool legsBusted = false;
    private bool crawling = false;
    private float movementSpeed = 0;
    private float oscillator = 0;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        crawling = false;

        agent.destination = transform.position;

        if (animator == null) // The visual model of the enemy are gameObjects attached to the bones, so the animator is placed in a children
        {
            animator = GetComponentInChildren<Animator>();
        }

        UpdateState(EnemyState.Idle);
    }

    #region - State Machine Logic -

    void FixedUpdate()
    {
        float targetDistance = Vector2.Distance(target.position, transform.position);
        if(!dead)
        {
            if (enemyState != EnemyState.Crawl && crawling)
            {
                UpdateState(EnemyState.Crawl);
            }
            else if (proximityRange > targetDistance && enemyState != EnemyState.Chase && !crawling)
            {
                UpdateState(EnemyState.Chase);
            }
            else if (proximityRange < targetDistance && enemyState != EnemyState.Idle && !crawling)
            {
                UpdateState(EnemyState.Idle);
            }
        }
        
    }

    public void LegsBusted()
    {
        crawling = true;
    }

    public void EnemyKilled()
    {
        UpdateState(EnemyState.Dead);
    }

    private void UpdateState(EnemyState state)
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

    private IEnumerator Idle() // Clears Target
    {
        agent.destination = transform.position;
        animator.SetFloat("Speed", 0);

        yield return null;
    }

    private IEnumerator Chase()
    {
        agent.speed = 1.0f;
        while (true) // Asings a target and interpolates a value between 0 and 1 in order to match the animation speed that is inconsistent
        {
            agent.destination = target.position;

            movementSpeed = movementSpeed + 0.011f;
            animator.SetFloat("Speed", Mathf.Lerp(0, 0.5f, movementSpeed));

            oscillator = Mathf.PingPong(Time.time * modifierValue, 1);
            agent.speed = oscillator;

            yield return null;
        }
    }

    private IEnumerator Crawl()
    {
        if (!legsBusted) // When this state is called for the first time, plays the falling animation
        {

            legsBusted = true;
            animator.SetTrigger("Fall");
            animator.SetBool("Crawling", true);

            yield return new WaitForSeconds(3);
        }
        //else

        agent.speed = 0.5f;

        while (true)
        {
            agent.destination = target.position;



            yield return null;
        }

    }

    private IEnumerator Dead()
    {
        dead = true;
        animator.SetTrigger("Die");
        agent.enabled = false;
        
        yield return null;
    }

    private IEnumerator FallingAnimationTimming()
    {
        yield return null;
    }

    #endregion
}

public enum EnemyState
{
    Idle,
    Chase,
    Crawl,
    Dead
}
