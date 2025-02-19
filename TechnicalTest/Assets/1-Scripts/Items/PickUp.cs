using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private ParticleSystem powerUpParticles;
    [SerializeField] private Transform model;
    [SerializeField] private AudioClip pickUpAudio;

    private AudioSource audioSource;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.Play();
    }

    private void FixedUpdate() // animate the 3D model
    {
        model.position = new Vector3(model.position.x, ((Mathf.PingPong(Time.time/2, 0.5f) + 4 ) / 4) + originalPosition.y, model.position.z);
        model.rotation = Quaternion.Euler(new Vector3(60 , Time.fixedTime * 50, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Weapon>() != null)
        {
            StartCoroutine(PowerUpWeapon(other.GetComponent<Weapon>()));
        }
    }

    public IEnumerator PowerUpWeapon(Weapon weapon) // Tell the weapon to Update his state
    {
        // feedback
        audioSource.clip = pickUpAudio;
        audioSource.Play();
        powerUpParticles.Play();

        weapon.AmmoSwap(true);
        model.gameObject.SetActive(false);

        yield return new WaitForSeconds(4);

        Destroy(gameObject);
    }
}
