using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //Singleton

    [SerializeField] private EnemyHealth[] enemies;

    public int deadZombies;
    private PlayerHealth PlayerHealth;
    private SceneLoaderDoor door;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerHealth = PlayerHealth.Instance;
        door = SceneLoaderDoor.Instance;
    }

    private void AllZombiesKilled()
    {
        door.OpenExit();
    }


}
