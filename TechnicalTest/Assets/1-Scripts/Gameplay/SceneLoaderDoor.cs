using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderDoor : MonoBehaviour
{
    public static SceneLoaderDoor Instance;

    public int sceneIndex;
    public Image whiteFlashPanel;

    public Color fadeInColor;
    public Color fadeOutColor;

    private Collider colliderSelf;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        colliderSelf = GetComponent<Collider>(); // Deactivates Collider to prevent the player from exiting the level before meeting the conditions
        if (sceneIndex == 0)
        {
            colliderSelf.enabled = false;
            whiteFlashPanel.gameObject.SetActive(true);
        }

        if (whiteFlashPanel.color == fadeInColor) // Do a Fade in Transition to make up loadingScreens
        {
            whiteFlashPanel.DOColor(fadeOutColor, 3).OnComplete(() => { DOTween.Kill(whiteFlashPanel); });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealth>() != null)
        {
            StartCoroutine(WhiteFlashAndLoadScene(sceneIndex));
            AimController.Instance.LockOrUnlockMouse();
        }
    }

    public void ManualyLoadScene(int index) // Called in Buttons Unity Events
    {
        StartCoroutine(WhiteFlashAndLoadScene(index));
    }

    public void OpenExit() // Enable Collider and Mesh for the player to use
    {
        colliderSelf.enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    private IEnumerator WhiteFlashAndLoadScene(int index) // Transition to make up loading screens
    {
        whiteFlashPanel.gameObject.SetActive(true);
        whiteFlashPanel.DOColor(fadeInColor, 1).OnComplete(() => { DOTween.Kill(whiteFlashPanel); });

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(index);
    }
}
