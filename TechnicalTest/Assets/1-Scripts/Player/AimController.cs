using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
    public static AimController Instance;

    [SerializeField] private bool mouseLocked = true;
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private float radious = 15;
    [SerializeField, Range(0,0.2f)] private float speed;
    [SerializeField] private Vector2 offset;

    private Vector2 aimPosition;
    private Camera mainCamera;
    private Transform aimObject;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCamera = CameraReferences.Instance.playerCamera;
        aimObject = CameraReferences.Instance.aimObject;

        if (mouseLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void FixedUpdate()
    {
        RotateAim(aimObject, aimPosition, radious, speed);

        // Convert AimObj's world position to screen coordinates
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(aimObject.position);

        // Check if AimObj is in front of the camera
        if (screenPosition.z > 0)
        {
            // Update crosshair position in UI space
            crosshairUI.position = screenPosition + new Vector3(offset.x, offset.y);
        }
    }        

    public void OnAim(InputAction.CallbackContext context) // Reads the value of the mouse movement
    {
        if (mouseLocked)
        {
            aimPosition = context.ReadValue<Vector2>();
        }        
    }

    public void LockOrUnlockMouse() // Changes state of the cursor in order to use Menu Interfaces
    {
        if (!mouseLocked)
        {
            mouseLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            mouseLocked = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void RotateAim(Transform aimObj, Vector2 aimDelta, float radius, float sensitivity)
    {
        // Convert mouse movement to angles
        float yaw = aimDelta.x * sensitivity; // Horizontal movement (Y-axis in 3D space)
        float pitch = -aimDelta.y * sensitivity; // Vertical movement (X-axis in 3D space)

        // Get the current direction of AimObj relative to the player
        Vector3 direction = (aimObj.position - transform.position).normalized;

        // Apply rotation on both axes
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up); // Rotation around the Y-axis
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, transform.right); // Rotation around the X-axis

        // Combine both rotations
        direction = yawRotation * pitchRotation * direction;

        // Maintain a constant distance from the player
        aimObj.position = transform.position + direction * radius;
    } 
}
