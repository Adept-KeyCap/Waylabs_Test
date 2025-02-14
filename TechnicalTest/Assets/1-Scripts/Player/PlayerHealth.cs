using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [SerializeField] private Slider healthSlider;

    private float health = 100;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health;

        if (health < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GetComponent<Rigidbody>().freezeRotation = false;
    }

    
}
