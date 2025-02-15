using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public AudioClip[] hurtAudios;
    public AudioClip deathAudio;

    [SerializeField] private Slider healthSlider;

    private float health = 100;
    private AudioSource audioSource;

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
        audioSource.clip = deathAudio;
        audioSource.Play();
        GetComponent<Rigidbody>().freezeRotation = false;
    }

    
}
