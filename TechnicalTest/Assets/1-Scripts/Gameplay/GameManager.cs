using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //Singleton

    [SerializeField] private EnemyHealth[] enemies;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private TMP_Text remainingEnemies;
    [SerializeField] private AudioClip victorySound;

    public int deadEnemies;
    private PlayerHealth PlayerHealth;
    private SceneLoaderDoor door;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        remainingEnemies.text = enemies.Length.ToString() + " / " + enemies.Length.ToString();
    }

    private void Start()
    {
        PlayerHealth = PlayerHealth.Instance;
        door = SceneLoaderDoor.Instance;
        
        audioSource = GetComponent<AudioSource>();
    }

    public void IncreaseKillCount()
    {
        deadEnemies++;

        remainingEnemies.text = (enemies.Length - deadEnemies).ToString() + " / " + enemies.Length;

        if(deadEnemies >= enemies.Length)
        {
            AllEnemiesKilled();
        }
    }

    private void AllEnemiesKilled()
    {
        door.OpenExit();
        audioSource.clip = victorySound;
        audioSource.loop = false;
        audioSource.Play();
        missionPanel.SetActive(false);
        victoryPanel.SetActive(true);
    }


}
