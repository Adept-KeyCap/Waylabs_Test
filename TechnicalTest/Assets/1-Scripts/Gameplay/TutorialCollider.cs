using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    public GameObject deactivateThis;
    public GameObject activateThis;

    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        deactivateThis.SetActive(false);
        activateThis.SetActive(true);
        audioSource.Play();
        gameObject.SetActive(false);
    }

}
