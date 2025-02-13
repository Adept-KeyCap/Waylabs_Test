using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour, IHittable
{
    public float health;
    public EnemyHealth enemyHealth;

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void OnHit(Vector3 hitPoint, float damage, Vector3 hitForce)
    {

        Debug.Log(gameObject.name + "Was Hit!");
        enemyHealth.StackDamage(damage);
        health = health - damage;

        if (health < 0)
        {
            //Play blood particle system
            enemyHealth.DropBlood(transform);
            gameObject.SetActive(false);
        }
    }

}
