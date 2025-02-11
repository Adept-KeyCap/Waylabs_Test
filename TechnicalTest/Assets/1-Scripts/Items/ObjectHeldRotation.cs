using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectHeldRotation : MonoBehaviour
{
    [SerializeField] private Transform aimObject;
    [SerializeField] float adjustmentSpeed = 5f;
    [SerializeField, Range(0, 1)] private float movementRangeX = 0.5f;
    [SerializeField, Range(0, 1)] private float movementRangeY = 0.5f;

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
