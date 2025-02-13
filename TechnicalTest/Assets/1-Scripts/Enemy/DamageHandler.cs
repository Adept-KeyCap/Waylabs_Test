using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour, IHittable
{
    [Header("Stats")]
    public float health;

    [Header("Body Parts")]
    public bool head;
    public bool leg;

    [Header("References")]
    public EnemyHealth enemyHealth;
    public GameObject parentOf;


    public void OnHit(Vector3 hitPoint, float damage, Vector3 hitForce)
    {
        if (head)
        {
            damage = damage * 3;
        }

        Debug.Log(gameObject.name + "Was Hit!");
        enemyHealth.StackDamage(damage);
        health = health - damage;

        if (health < 0 && parentOf == null)
        {
            CheckForLegs();

            enemyHealth.DropBlood(transform);
            gameObject.SetActive(false);
        }
        else if (health < 0 && parentOf != null)
        {
            CheckForLegs();

            enemyHealth.DropBlood(transform);
            parentOf.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void CheckForLegs()
    {
        if (leg)
        {
            enemyHealth.OneLegless();
        }
    }

}
