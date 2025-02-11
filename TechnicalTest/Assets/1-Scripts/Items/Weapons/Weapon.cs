using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private GameObject prefabProjectile; // For Physical Bullets
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxAmmo;
    [SerializeField] private float projectileSpeed = 400.0f;
    [SerializeField, Range(1,5)] private float reloadTime;

    [Header("Modifiers")]
    [SerializeField] private bool automatic;
    [SerializeField] private bool laser;
    [SerializeField] private TMP_Text ammoTxt; 
    [SerializeField] private LayerMask mask;
    [SerializeField] private Transform socketEjection; //Bullets Casting Particle System
    [SerializeField] private Vector2 shotOffset;
    [SerializeField] private Transform positionCrosshair;


    [Header("Audio Clips")]
    [SerializeField] private AudioClip audioClipReload;
    [SerializeField] private AudioClip audioClipFire;
    [SerializeField] private AudioClip audioClipFireEmpty;


    private float currentAmmo;
    private Vector3 rayDirection;

    private Camera mainCamera;
    private Transform aimObj;
    private GameObject mainCrosshair;

    void Start()
    {
        mainCamera = CameraReferences.Instance.playerCamera;
        aimObj = CameraReferences.Instance.aimObject;
        mainCrosshair = CameraReferences.Instance.mainCrosshair;
        currentAmmo = maxAmmo;

        ammoTxt.text = currentAmmo.ToString() + "|" + maxAmmo;
        //AmmoDisplay(false);
    }

    private void FixedUpdate()
    {
        rayDirection = (aimObj.position - mainCamera.transform.position).normalized - new Vector3(shotOffset.x, shotOffset.y);
        if (Physics.Raycast(new Ray(firePoint.position, rayDirection), out RaycastHit hit, 500, mask));
        {
            positionCrosshair.position = hit.point;
        }
    }

    public void Fire(float spreadMultiplier = 1.0f)
    {
        Debug.LogWarning("Weapon Fired");

        // Reduce ammo
        currentAmmo = Mathf.Clamp(currentAmmo - 1, 0, maxAmmo);
        ammoTxt.text = currentAmmo.ToString() + "|" + maxAmmo;

        Quaternion rotation;

        // 🔹 Raycast to check if the shot hits anything
        if (Physics.Raycast(new Ray(firePoint.position, rayDirection), out RaycastHit hit, 500, mask))
        {
            // ✅ Aim at the hit point
            rotation = Quaternion.LookRotation(hit.point - firePoint.position);
        }
        else
        {
            // ✅ No hit → Shoot straight
            rotation = Quaternion.LookRotation(rayDirection);
        }

        if (!laser)
        {
            // ✅ Create and launch the projectile
            GameObject projectile = Instantiate(prefabProjectile, firePoint.position, rotation);
            projectile.GetComponent<Bullet>().GetWeaponStat(projectileSpeed);
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
        }
    }


    public void AmmoDisplay(bool logic)
    {
        switch (logic)
        {
            case false:
                ammoTxt.gameObject.transform.parent.gameObject.SetActive(false);
                positionCrosshair.gameObject.SetActive(false);
                mainCrosshair.gameObject.SetActive(true);
                break;

            case true:
                ammoTxt.gameObject.transform.parent.gameObject.SetActive(true);
                positionCrosshair.gameObject.SetActive(true);
                mainCrosshair.gameObject.SetActive(false);
                break;
        }
    }

    public void Reload()
    {
        StartCoroutine(ReloadCorutine(reloadTime));
    }

    private IEnumerator ReloadCorutine(float time)
    {
        Debug.Log("Reloading");

        //Play Reload Audio

        yield return new WaitForSeconds(time);

        currentAmmo = maxAmmo;
    }
}
