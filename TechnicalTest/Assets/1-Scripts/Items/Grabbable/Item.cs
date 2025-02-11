using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private GameObject highlightObj;
    [SerializeField, Range(0,10)] private float forceScaler;

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
        grabbed = true;
        transform.parent = pos;
        rb.isKinematic = true;
        collider.enabled = false;
        transform.localPosition = Vector3.zero;
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
}
