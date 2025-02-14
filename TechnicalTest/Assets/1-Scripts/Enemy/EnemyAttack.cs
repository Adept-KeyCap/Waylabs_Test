using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage;

    private float attackCooldown = 2; 
    private float lastAttackTime = 0;   
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = PlayerHealth.Instance;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerHealth>() == playerHealth)
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
