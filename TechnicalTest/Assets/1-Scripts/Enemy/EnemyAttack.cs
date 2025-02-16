using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage;

    private float attackCooldown = 1.6f; 
    private float lastAttackTime = 0;   
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = PlayerHealth.Instance;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealth>() == playerHealth)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy Attacks Player!");
        playerHealth.TakeDamage(attackDamage);
    }
}
