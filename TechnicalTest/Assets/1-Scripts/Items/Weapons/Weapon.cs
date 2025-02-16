using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Weapon : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private GameObject prefabProjectile; // For Physical Bullets
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxAmmo;
    [SerializeField] private float projectileSpeed = 400.0f;
    [SerializeField, Range(0.1f, 1.0f)] private float fireRate = 0.2f; // Fire rate (bullets per second)
    [SerializeField, Range(1, 5)] private float reloadTime;

    [Header("Modifiers")]
    [SerializeField] private bool automatic; // Determines if the weapon is full-auto
    [SerializeField] private TMP_Text ammoTxt;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Vector2 shotOffset;
    [SerializeField] private Transform positionCrosshair;

    [Header("Laser")]
    [SerializeField] private bool laser;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [Tooltip("0 = Standard Material \n1 = Laser Material")]
    [SerializeField] private List<Material> materials;
 
    [Header("Feedback")]
    [SerializeField] private ParticleSystem bulletParticles;
    [SerializeField] private ParticleSystem castingParticles;
    [SerializeField] private ParticleSystem laserParticles;

    private float currentAmmo;
    private bool canFire = true; // Used to control semi-auto firing
    private bool isFiring = false; // Tracks if auto-fire is active
    private Vector3 rayDirection;

    private Camera mainCamera;
    private Transform aimObj;
    private GameObject mainCrosshair;
    private LineRenderer lineRenderer;
    private WeaponSoundManager weaponSoundManager;

    void Start()
    {
        mainCamera = CameraReferences.Instance.playerCamera;
        aimObj = CameraReferences.Instance.aimObject;
        mainCrosshair = CameraReferences.Instance.mainCrosshair;
        lineRenderer = GetComponent<LineRenderer>();
        weaponSoundManager = GetComponent<WeaponSoundManager>();

        currentAmmo = maxAmmo;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        ammoTxt.text = $"{currentAmmo} | {maxAmmo}";
    }

    private void FixedUpdate() // Constantly Updates the 3D Crosshair Position
    {
        rayDirection = (aimObj.position - mainCamera.transform.position).normalized - new Vector3(shotOffset.x, shotOffset.y);

        if (Physics.Raycast(new Ray(firePoint.position, rayDirection), out RaycastHit hit, 500, mask))
        {
            positionCrosshair.position = hit.point + new Vector3(0, 0, 0.05f);
            // Get the surface normal
            Vector3 surfaceNormal = hit.normal;

            // Align object’s direction with the surface normal
            positionCrosshair.transform.forward = surfaceNormal;
        }
    }
    private void OnDisable() // Changes Crosshairs when disabled
    {
        if (mainCrosshair != null)
            mainCrosshair.gameObject.SetActive(true);
    }

    public void StartFire(bool isPressed) // checker method to change weapon shooting behavior: Automatic or Semi-Automatic
    {
        if (isPressed)
        {
            if (automatic)
            {
                if (!isFiring) // Prevent multiple invokes
                {
                    isFiring = true;
                    InvokeRepeating(nameof(Fire), 0f, fireRate);
                }
            }
            else
            {
                Fire(); // Semi-auto fires once per click
            }
        }
        else
        {
            StopFiring(); // Stops automatic fire when released
        }
    }

    public void StopFiring()
    {
        if (automatic)
        {
            isFiring = false;
            CancelInvoke(nameof(Fire)); // Stops automatic fire
        }
    }

    #region - New Input System -
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started) // Button pressed
        {
            if (automatic)
            {
                isFiring = true;
                InvokeRepeating(nameof(Fire), 0f, fireRate);
            }
            else
            {
                Fire(); // Semi-auto fires once
            }
        }
        else if (context.canceled) // Button released
        {
            if (automatic)
            {
                isFiring = false;
                CancelInvoke(nameof(Fire));
            }
        }
    }
    #endregion

    private void Fire() // If the weapon have available ammo, shoot and refresh the ammo display 
    {
        if (!canFire ) //checks for fire timing (Semi-Automatic Fire)
        {
            return;
        }
        else if(currentAmmo <= 0)
        {
            weaponSoundManager.Play_FireEmpty();
            return;
        }

        // Reduce ammo and refresh display
        currentAmmo = Mathf.Clamp(currentAmmo - 1, 0, maxAmmo);
        ammoTxt.text = $"{currentAmmo} | {maxAmmo}";

        Quaternion rotation;

        // Raycast to check if the shot hits anything
        if (Physics.Raycast(new Ray(firePoint.position, rayDirection), out RaycastHit hit, 500, mask))
        {
            // Aim at the hit point
            rotation = Quaternion.LookRotation(hit.point - firePoint.position);
        }
        else
        {
            // No hit, then Shoot straight
            rotation = Quaternion.LookRotation(rayDirection);
        }

        if (!laser)  // Checks if this weapon has laser ammo enabled
        {
            // Shot feedback
            weaponSoundManager.Play_Fire();
            bulletParticles.Play();
            castingParticles.Play();

            // Create and launch the projectile
            GameObject projectile = Instantiate(prefabProjectile, firePoint.position, rotation);
            Debug.Log(projectile.name + " Fired");
            projectile.GetComponent<Bullet>().GetWeaponStat(projectileSpeed, projectileSpeed/2);
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
        }
        else 
        {    
            // Laser shot feedback
            weaponSoundManager.Play_Laser();
            laserParticles.Play();

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, positionCrosshair.position);
            StartCoroutine(ClearLaser(0.3f));

            if (hit.collider.gameObject.GetComponent<IHittable>() != null)
            {
                hit.collider.gameObject.GetComponent<IHittable>().OnHit(Vector3.zero, projectileSpeed * 1.5f, Vector3.zero);
                lineRenderer.SetPosition(1, hit.point);
            }
        }

        if (!automatic) // If semi-auto, prevent firing again until cooldown
        {
            canFire = false;
            Invoke(nameof(ResetFire), fireRate);
        }
    }

    public void AmmoDisplay(bool logic) // Decides whether to display current ammo, or not
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

    public void AmmoSwap(bool logic) // Decides how to display the weapon depending on the type of Ammo
    {
        switch (logic)
        {
            case false:
                laser = false;
                skinnedMeshRenderer.material = materials[0];
                break;

            case true:
                laser = true;
                skinnedMeshRenderer.material = materials[1];
                break;
        }
    }

    private void ResetFire()
    {
        canFire = true; // Allow next shot
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        // Feedback
        weaponSoundManager.Play_Reload();

        yield return new WaitForSeconds(reloadTime);
        ammoTxt.text = $"{currentAmmo} | {maxAmmo}";
        currentAmmo = maxAmmo;
    }

    private IEnumerator ClearLaser(float time) // Laser visuals are shown with a "LineRenderer" component, we have to disable it a bit after the shot was taken
    {
        yield return new WaitForSeconds(time);

        lineRenderer.enabled = false;
    }
}
