using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxSpeed = 5f;
    private Transform cameraTransform;
    private Rigidbody rb;
    private Vector2 moveInput;

    void Start()
    {
        cameraTransform = CameraReferences.Instance.playerCamera.transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        if (rb.velocity.magnitude <= maxSpeed)
        {

            rb.AddForce(move.normalized * speed, ForceMode.Impulse);

        }

        //character body rotation to camera rotation
        Vector3 eulerRotation = new Vector3(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(eulerRotation);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            maxSpeed *= 2;
        }
        else if (context.canceled)
        {
            maxSpeed = maxSpeed/2;
        }
    }
}
