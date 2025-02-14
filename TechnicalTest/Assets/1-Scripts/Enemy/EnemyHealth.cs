using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth: MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField, Range(1, 10)] private int forceMitigation = 1;

    [Header("References")]
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private Slider healthSlider;
 

    private DamageHandler[] bodyParts;
    private EnemyStateMachine stateMachine;
    private int bustedLegs;

    void Start()
    {
        bodyParts = GetComponentsInChildren<DamageHandler>();
        stateMachine = GetComponent<EnemyStateMachine>();

        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthSlider.enabled = false;    

        foreach (DamageHandler handler in bodyParts)
        {
            handler.enemyHealth = this;
        }

    }

    public void StackDamage(float damage)
    {
        health = health - damage;
        healthSlider.enabled = false;
        DisplayHealth(health);

        if (health < 0)
        {
            healthSlider.gameObject.SetActive(false);
            stateMachine.EnemyKilled();
        }
    }

    private void DisplayHealth(float value)
    {
        healthSlider.value = value;
    }


    public void DropBlood(Transform position)
    {
        // Instantiate at the position of the given Transform
        GameObject blood = Instantiate(bloodParticles, position.position, Quaternion.identity);

        // Get and Play the Particle System
        ParticleSystem ps = blood.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
        else
        {
            Debug.LogError("No ParticleSystem found on the instantiated object!");
        }
    }


    public void OneLegless()
    {
        bustedLegs++;
        if (bustedLegs >= 2)
        {
            //Call the Enemy Animator to triggrer the new animation
            stateMachine.LegsBusted();
            Debug.LogWarning(gameObject.name + "Both Legs Busted!");
        }
    }

}
