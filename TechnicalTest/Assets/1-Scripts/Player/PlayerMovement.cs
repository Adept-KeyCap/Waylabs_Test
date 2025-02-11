using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
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
        
        rb.AddForce(move.normalized * speed, ForceMode.VelocityChange);

        //character body rotation to camera rotation
        Vector3 eulerRotation = new Vector3(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(eulerRotation);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
