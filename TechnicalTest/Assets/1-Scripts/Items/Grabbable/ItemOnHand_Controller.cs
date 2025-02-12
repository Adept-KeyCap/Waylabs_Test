using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemOnHand_Controller : MonoBehaviour
{
    [Header("Item Detection")]
    public Item detected_Item;
    public Item held_Item;

    [Header("Grab Attributes")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabDistance = 3f;  // Maximum grab distance

    private Transform aimObj;
    private Transform playerCamera;
    private bool itemOnHand = false;
    private Item hitObject;

    private float pressStartTime = 0f;
    private bool isHolding = false;

    private void Start()
    {
        aimObj = CameraReferences.Instance.aimObject.transform;
        playerCamera = CameraReferences.Instance.playerCamera.transform;
    }

    void FixedUpdate()
    {
        // Corrected ray direction
        Vector3 rayDirection = (aimObj.position - playerCamera.position).normalized;

        // Perform the Raycast
        if (Physics.Raycast(playerCamera.position, rayDirection, out RaycastHit hit, grabDistance))
        {
            detected_Item = hit.collider.GetComponent<Item>(); // Get Item component

            if (detected_Item != null) // If an Item is detected
            {
                // If it's a new item, update the reference and highlight it
                if (hitObject != detected_Item)
                {
                    ResetHighlight();  // Reset previous highlight
                    hitObject = detected_Item;
                    hitObject.Highlight(true);
                }

                itemOnHand = true;  // knows if an item is available to grab
            }
            else
            {
                ResetHighlight();
            }
        }
        else
        {
            ResetHighlight();
        }


        if (isHolding)
        {
            float currentHoldTime = Time.time - pressStartTime;
            Debug.Log($"Holding Q for {currentHoldTime:F2} seconds...");
        }
    }

    #region General Items Methods
    public void GrabItem()
    {
        if (itemOnHand && hitObject != null)
        {
            hitObject.Grabbed(grabPoint);
            held_Item = hitObject;
        }
        else
        {
            Debug.LogWarning("No Item to Grab!");
        }
    }

    private void ResetHighlight()
    {
        if (hitObject != null)
        {
            hitObject.Highlight(false);
            hitObject = null;
        }
        itemOnHand = false;  // ✅ Ensure it resets when no item is detected
    }

    public void OnHoldQ(InputAction.CallbackContext context)
    {
        if (context.started) // Q Button is pressed
        {
            pressStartTime = Time.time;
            isHolding = true;
            Debug.Log("Q key pressed.");
        }
        else if (context.canceled) // Q Button is released
        {
            float holdDuration = Time.time - pressStartTime;
            Debug.Log($"Q key was held for {holdDuration:F2} seconds.");
            isHolding = false;
            held_Item.ThrowItem(holdDuration);
            held_Item = null;
        }
    }
    #endregion

    #region Weapon Methods

    public void FireWeapon(InputAction.CallbackContext context)
    {
        if (held_Item != null && held_Item.TryGetComponent(out Weapon weapon))
        {
            if (context.performed) // Button is held down
            {
                weapon.StartFire(true);
            }
            else if (context.canceled) // Button is released
            {
                weapon.StartFire(false);
            }
        }
    }



    public void ReloadWeapon()
    {
        if (held_Item != null && held_Item.TryGetComponent(out Weapon weapon))
        {
            weapon.Reload();
        }
        else
        {
            return;
        }
    }

    #endregion

}
