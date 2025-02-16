using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth: MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float health;

    [Header("References")]
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject damageDealer;

    [Header("Audio")]
    [SerializeField] private AudioClip damageAudio;
    [SerializeField] private AudioClip bloodAudio;

    private DamageHandler[] bodyParts;
    private EnemyStateMachine stateMachine;
    private GameManager gameManager;
    private AudioSource audioSource;
    private int bustedLegs;
    private bool dead = false;

    void Start()
    {
        gameManager = GameManager.Instance;

        bodyParts = GetComponentsInChildren<DamageHandler>();
        stateMachine = GetComponent<EnemyStateMachine>();
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = damageAudio;

        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(false);

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

        audioSource.clip = damageAudio;
        audioSource.Play();

        if (health < 0 && !dead)
        {
            dead = true;
            damageDealer.SetActive(false);

            foreach(DamageHandler handler in bodyParts)
            {
                handler.gameObject.GetComponent<Collider>().enabled = false;
            }

            healthSlider.gameObject.SetActive(false);
            stateMachine.EnemyKilled();
            gameManager.IncreaseKillCount();
        }
    }

    public void DropBlood(Transform position)
    {
        // Instantiate at the position of the given Transform
        GameObject blood = Instantiate(bloodParticles, position.position, Quaternion.identity);
        audioSource.clip = bloodAudio;
        audioSource.Play();

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
    private void DisplayHealth(float value)
    {
        healthSlider.gameObject.SetActive(true);
        healthSlider.value = value;
    }

    public void OneLegless()
    {
        bustedLegs++;
        if (bustedLegs >= 2)
        {
            //Call the Enemy Animator to triggrer the new animation
            stateMachine.LegsBusted();
        }
    }

}
