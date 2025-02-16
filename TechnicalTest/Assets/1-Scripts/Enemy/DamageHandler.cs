using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour, IHittable // Weapon shots can affect this object, "IHittable" interface implemented
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
        if (head) // triple the amount of damage if the head gets hit
        {
            damage = damage * 3;
        }

        // decrease global health
        enemyHealth.StackDamage(damage);
        health = health - damage;

        if (health < 0 && parentOf == null)
        {
            CheckForLegs();

            enemyHealth.DropBlood(transform);
            gameObject.SetActive(false);
        }
        else if (health < 0 && parentOf != null) // if any other object depends on the position of this one, disable it too
        {
            CheckForLegs();

            enemyHealth.DropBlood(transform);
            parentOf.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void CheckForLegs() // If this DamageHandler is checked as a leg, notify the global health
    {
        if (leg)
        {
            enemyHealth.OneLegless();
        }
    }

}
