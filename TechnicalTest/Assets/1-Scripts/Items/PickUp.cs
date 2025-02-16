using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private ParticleSystem powerUpParticles;
    [SerializeField] private Transform model;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }

    private void FixedUpdate()
    {
        model.position = new Vector3(model.position.x, (Mathf.PingPong(Time.time/2, 0.5f) + 4 ) / 4, model.position.z);
        model.rotation = Quaternion.Euler(new Vector3(60 , Time.fixedTime * 50, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("gameObject Inside: " + other.GetComponent<Weapon>());
        if (other.GetComponent<Weapon>() != null)
        {
            StartCoroutine(PowerUpWeapon(other.GetComponent<Weapon>()));
        }
    }

    public IEnumerator PowerUpWeapon(Weapon weapon)
    {
        Debug.LogWarning("Powering Up Weapon");
        audioSource.Play();
        powerUpParticles.Play();
        weapon.AmmoSwap(true);
        model.gameObject.SetActive(false);

        yield return new WaitForSeconds(4);

        Destroy(gameObject);
    }
}
