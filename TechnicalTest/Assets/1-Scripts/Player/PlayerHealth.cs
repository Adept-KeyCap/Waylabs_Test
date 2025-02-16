using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private DefeatScreen defeatScreen;

    public AudioClip[] hurtAudios;
    public AudioClip deathAudio;

    private float health = 100;
    private AudioSource audioSource;
    private bool dead;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health;
        int randormNum = Random.Range(0,hurtAudios.Length);
        audioSource.clip = hurtAudios[randormNum];
        audioSource.Play();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!dead)
        {
            dead = true;

            audioSource.clip = deathAudio;
            audioSource.Play();
            defeatScreen.gameObject.SetActive(true);

            GetComponent<Rigidbody>().freezeRotation = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<AimController>().LockOrUnlockMouse();
            GetComponent<AimController>().enabled = false;
        }

    }

    
}
