using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour, IHittable
{
    [SerializeField] private GameObject highlightObj;
    [SerializeField, Range(0,10)] private float forceScaler;
    [SerializeField] private Vector3 customRotation;

    private Rigidbody rb;
    private Collider collider;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private bool grabbed = false;
    private Camera camera;
    private Transform aimObj;

    void Start()
    {
        camera = CameraReferences.Instance.playerCamera;
        aimObj = CameraReferences.Instance.aimObject;

        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        collider = GetComponent<Collider>();

        highlightObj.GetComponent<MeshFilter>().mesh = mesh;
        highlightObj.SetActive(false);
    }

    public void Grabbed(Transform pos)
    {
        if(gameObject.GetComponent<Weapon>() != null)
        {
            gameObject.GetComponent<Weapon>().AmmoDisplay(true);
        }
        else
        {
            Debug.Log("This is not a Weapon");
            return;
        }

        grabbed = true;
        transform.parent = pos;
        transform.localRotation = Quaternion.Euler(customRotation);
        rb.isKinematic = true;
        collider.enabled = false;
        transform.localPosition = Vector3.zero;

        CheckForWeapon(grabbed);
    }

    public void ThrowItem(float addedForce)
    {

        Vector3 forceVec = (aimObj.transform.position - camera.transform.position) * (addedForce * forceScaler);


        if (grabbed)
        {
            rb.isKinematic = false;
            collider.enabled = true;
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


    public void OnHit(Vector3 hitPoint, Vector3 hitForce)
    {
        rb.AddForce(hitForce, ForceMode.Impulse);
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
