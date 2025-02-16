using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //Singleton

    [SerializeField] private EnemyHealth[] enemies;
    [SerializeField] private Transform[] ammoSpawnPoint;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private TMP_Text remainingEnemies;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private GameObject ammoItemPrefab;

    public int deadEnemies;
    private PlayerHealth PlayerHealth;
    private SceneLoaderDoor door;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        remainingEnemies.text = enemies.Length.ToString() + " / " + enemies.Length.ToString(); // Displays how many enemies remain
    }

    private void Start()
    {
        PlayerHealth = PlayerHealth.Instance;
        door = SceneLoaderDoor.Instance;
        
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(RandomAmmoSpawn()); // Spawns a laser Ammo pick up
    }

    public void IncreaseKillCount() // Update the number of remaining enemies and display it
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

    private IEnumerator RandomAmmoSpawn() // Chooses a random time to spawn a Laser Ammo PickUp
    {
        int randTime = Random.Range(60, 120);

        yield return new WaitForSeconds(randTime);

        int rand = Random.Range(0, ammoSpawnPoint.Length);
        Instantiate(ammoItemPrefab, ammoSpawnPoint[rand].position, Quaternion.identity);
    }

}
