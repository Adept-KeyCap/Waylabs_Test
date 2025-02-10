using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform aimObject;
    [SerializeField] private float radious = 15;
    [SerializeField, Range(0,0.2f)] private float speed;

    private Vector2 aimPosition;

    void Start()
    {
        //mainCamera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
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
            crosshairUI.position = screenPosition;
        }
    }        

    public void onAim(InputAction.CallbackContext context)
    {
        aimPosition = context.ReadValue<Vector2>();
    }

    void RotateAim(Transform aimObj, Vector2 aimDelta, float radius, float sensitivity)
    {
        // Convertimos el movimiento del mouse en ángulos
        float yaw = aimDelta.x * sensitivity; // Movimiento horizontal (Y en espacio 3D)
        float pitch = -aimDelta.y * sensitivity; // Movimiento vertical (X en espacio 3D)

        // Obtenemos la dirección actual del AimObj respecto al jugador
        Vector3 direction = (aimObj.position - transform.position).normalized;

        // Aplicamos la rotación en ambos ejes
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up); // Rotación en Y
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, transform.right); // Rotación en X

        // Combinamos las rotaciones
        direction = yawRotation * pitchRotation * direction;

        // Mantenemos la distancia constante
        aimObj.position = transform.position + direction * radius;
    }


}
