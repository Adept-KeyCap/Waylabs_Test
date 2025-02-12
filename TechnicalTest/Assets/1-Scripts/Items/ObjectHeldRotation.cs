using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectHeldRotation : MonoBehaviour
{
    [SerializeField] private Transform aimObject;

    private Vector2 aimDelta;
    private Vector3 handCenter;

    void Start()
    {

    }

    void FixedUpdate()
    {
        // Make the object face the aim object
        transform.LookAt(aimObject);        
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimDelta = context.ReadValue<Vector2>(); // Read mouse movement input
    }
}
