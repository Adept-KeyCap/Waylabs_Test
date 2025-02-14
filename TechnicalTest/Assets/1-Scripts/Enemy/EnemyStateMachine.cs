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
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    [Header("Stats")]
    [SerializeField] private float proximityRange;
    [SerializeField] private float attackRange;
    [SerializeField, Range(1, 2)] private float modifierValue;

    [Header("References")]
    [SerializeField] private Animator animator;


    private bool inRange;
    private bool legsBusted = false;
    private bool crawling = false;
    private float movementSpeed = 0;
    private float oscillator = 0;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        crawling = false;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void FixedUpdate()
    {
        float targetDistance = Vector2.Distance(target.position, transform.position);

        if (enemyState != EnemyState.Crawl && crawling)
        {
            UpdateState(EnemyState.Crawl);
        }
        else if (proximityRange > targetDistance && enemyState != EnemyState.Chase && !crawling)
        {
            inRange = true;
            UpdateState(EnemyState.Chase);
        }
        else if(proximityRange < targetDistance && enemyState != EnemyState.Idle && !crawling)
        {
            inRange = false;
            UpdateState(EnemyState.Idle);
        }
        else if (attackRange < targetDistance && enemyState != EnemyState.Attack)
        {
            UpdateState(EnemyState.Attack);
        }
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

    private IEnumerator Idle()
    {

        agent.destination = transform.position;
        animator.SetFloat("Speed", 0);

        yield return null;

    }

    private IEnumerator Chase()
    {
        agent.speed = 1.0f;
        while (true)
        {
            agent.destination = target.position;

            movementSpeed = movementSpeed + 0.011f;
            animator.SetFloat("Speed", Mathf.Lerp(0, 0.5f, movementSpeed));

            oscillator = Mathf.PingPong(Time.time * modifierValue, 1);
            agent.speed = oscillator;

            yield return null;
        }
    }

    private IEnumerator Attack()
    {

        agent.speed = 0;
        animator.SetFloat("Speed", 0);

        yield return null;

    }

    private IEnumerator Crawl()
    {
        Debug.Log("Enemy Crawlling");

        if (!legsBusted)
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
        yield return null;
    }

    private IEnumerator FallingAnimationTimming()
    {
        yield return null;
    }


    public void LegsBusted()
    {
        crawling = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, proximityRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Crawl,
    Dead
}
