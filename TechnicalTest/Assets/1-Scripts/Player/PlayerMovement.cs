using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float jumpForce = 50f;

    private Transform cameraTransform;
    private Rigidbody rb;
    private Vector2 moveInput;

    void Start()
    {
        cameraTransform = CameraReferences.Instance.playerCamera.transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() // Move the player with physic forces
    {
        Move();
    }

    private void Move()
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

    public void OnJump(InputAction.CallbackContext context) // Method to give player the ability to jump, but not implemented for gameplay and balance reasons
    {
        if (context.performed)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnMove(InputAction.CallbackContext context) // Vector2 that stores the value from WASD movement Input
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) // Modifies the max speed the player can reach
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
