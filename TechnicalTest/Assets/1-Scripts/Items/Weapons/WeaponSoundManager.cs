using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip audioClipReload;
    [SerializeField] private AudioClip audioClipFire;
    [SerializeField] private AudioClip audioClipFireLaser;
    [SerializeField] private AudioClip audioClipFireEmpty;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play_Fire()
    {
        audioSource.clip = audioClipFire;
        audioSource.Play();
    }
    public void Play_Laser()
    {
        audioSource.clip = audioClipFireLaser;
        audioSource.Play();
    }
    public void Play_Reload()
    {
        audioSource.clip = audioClipReload;
        audioSource.Play();
    }
    public void Play_FireEmpty()
    {
        audioSource.clip = audioClipFireEmpty;
        audioSource.Play();
    }

}
