using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour, IHittable
{
    [SerializeField] private GameObject highlightObj;
    [SerializeField, Range(0,10)] private float forceScaler;
    [SerializeField] private Vector3 customRotation;
    [SerializeField] private AudioClip contactAudio;

    public InventoryObject inventoryObject;

    private Rigidbody rb;
    private Collider selfCollider;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private bool grabbed = false;
    private Camera playerCamera;
    private Transform aimObj;
    private AudioSource audioSource;

    void Start()
    {
        playerCamera = CameraReferences.Instance.playerCamera;
        aimObj = CameraReferences.Instance.aimObject;
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        selfCollider = GetComponent<Collider>();

        highlightObj.GetComponent<MeshFilter>().mesh = mesh;
        highlightObj.SetActive(false);

        audioSource.clip = contactAudio;
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
        if(collision.gameObject.GetComponent<DamageHandler>() != null && rb.velocity.magnitude >= 2)
        {
            DamageHandler handler = collision.gameObject.GetComponent<DamageHandler>();
            float damage = rb.velocity.magnitude * (rb.mass * 75);
            handler.OnHit(Vector3.zero, damage, Vector3.forward);
        }
    }

    public void Grabbed(Transform pos)
    {
        grabbed = true;
        transform.parent = pos;
        transform.localRotation = Quaternion.Euler(customRotation);
        rb.isKinematic = true;
        selfCollider.isTrigger = true;
        transform.localPosition = Vector3.zero;
        audioSource.Play();

        CheckForWeapon(grabbed);  
    }

    public void ThrowItem(float addedForce)
    {
        Vector3 forceVec = (aimObj.transform.position - playerCamera.transform.position) * (addedForce * forceScaler);

        selfCollider.isTrigger = false;

        if (grabbed)
        {
            rb.isKinematic = false;
            selfCollider.enabled = true;
            transform.parent = null;
            rb.AddForce(forceVec);
            grabbed = false;
            CheckForWeapon(grabbed);
        }
    }

    public void Highlight(bool logic)
    {
        if (logic && !grabbed)
        {
            highlightObj.SetActive(true);
        }
        else
        {
            highlightObj.SetActive(false);
        }
    }


    public void OnHit(Vector3 hitPoint, float damage, Vector3 hitForce)
    {
        rb.AddForce(hitForce/2, ForceMode.Impulse);
        Debug.Log(gameObject.name + "Was Hit!");
    }

    private void CheckForWeapon(bool logic)
    {
        if (gameObject.GetComponent<Weapon>() != null)
        {
            gameObject.GetComponent<Weapon>().AmmoDisplay(logic);
        }
    }


}
