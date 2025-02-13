using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth: MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private GameObject bloodParticles;

    [SerializeField, Range(1, 10)] private int forceMitigation = 1;


    private Rigidbody[] rb;
    private DamageHandler[] bodyParts;

    // Start is called before the first frame update
    void Start()
    {
        bodyParts = GetComponentsInChildren<DamageHandler>();
        rb = GetComponentsInChildren<Rigidbody>();

        foreach (DamageHandler handler in bodyParts)
        {
            handler.enemyHealth = this;
        }

        foreach (Rigidbody rbRef in rb)
        {
            rbRef.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StackDamage(float damage)
    {
        health = health - damage;
    }

    public void DropBlood(Transform position)
    {
        GameObject blood = Instantiate(bloodParticles, position);
        blood.gameObject.GetComponent<ParticleSystem>().Play();
    }

}
