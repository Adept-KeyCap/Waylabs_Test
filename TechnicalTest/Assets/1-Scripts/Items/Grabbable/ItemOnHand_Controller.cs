using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemOnHand_Controller : MonoBehaviour
{
    public static ItemOnHand_Controller Instance;

    [Header("Item Detection")]
    public Item detected_Item;
    public Item held_Item;

    [Header("Grab Attributes")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabDistance = 3f;  // Maximum grab distance

    private Transform aimObj;
    private Transform playerCamera;
    private bool grabbable = false;
    private Item hitObject;

    private float pressStartTime = 0f;
    private bool isHolding = false;

    [SerializeField] private InventoryManager invManager;
    [SerializeField] private List<GameObject> gameObjectsInInventory;


    private void Awake()
    {
        Instance = this;

        if (gameObjectsInInventory == null)
        {
            gameObjectsInInventory = new List<GameObject>(new GameObject[17]);
        }
    }

    private void Start()
    {
        aimObj = CameraReferences.Instance.aimObject.transform;
        playerCamera = CameraReferences.Instance.playerCamera.transform;

        if (invManager == null)
        {
            Debug.LogError("InventoryManager is not assigned to ItemOnHand_Controller!");
            return;
        }

        // Ensure the list is properly initialized
        gameObjectsInInventory = new List<GameObject>(new GameObject[invManager.inventorySlots.Length]);
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

                grabbable = true;  // knows if an item is available to grab
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
        if (grabbable && hitObject != null && held_Item == null)
        {
            grabbable = false;
            hitObject.Grabbed(grabPoint);
            held_Item = hitObject;

            // Ensure index is valid before assignment
            if (invManager.selectedSlot >= 0 && invManager.selectedSlot < gameObjectsInInventory.Count)
            {
                gameObjectsInInventory[invManager.selectedSlot] = held_Item.gameObject;
            }
            else
            {
                Debug.LogError("Invalid inventory slot index: " + invManager.selectedSlot);
            }
        }
        else
        {
            Debug.LogWarning("No Item to Grab!");
        }
    }

    public void SwapItem(int newItemId)
    {
        if (gameObjectsInInventory == null || newItemId < 0 || newItemId >= gameObjectsInInventory.Count)
        {
            Debug.LogWarning("SwapItem Error: gameObjectsInInventory is null or newItemId is out of range.");
            return;
        }

        // Deactivate the current held item before switching
        if (held_Item != null)
        {
            held_Item.gameObject.SetActive(false);
        }

        // Activate the new item if it exists
        if (gameObjectsInInventory[newItemId] != null)
        {
            gameObjectsInInventory[newItemId].SetActive(true);
            held_Item = gameObjectsInInventory[newItemId].GetComponent<Item>();
            if (held_Item.GetComponent<Weapon>() != null)
            {
                held_Item.GetComponent<Weapon>().AmmoDisplay(true);
            }
        }
        else
        {
            held_Item = null; // If the slot is empty, no item is held
        }
    }



    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabItem();
        }
        else if (context.performed) { }
        else if (context.canceled) { }
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
            invManager.GetSelectedObject(true);

            float holdDuration = Time.time - pressStartTime;
            Debug.Log($"Q key was held for {holdDuration:F2} seconds.");
            isHolding = false;
            held_Item.ThrowItem(holdDuration);
            held_Item = null;
        }
    }

    private void ResetHighlight()
    {
        if (hitObject != null)
        {
            hitObject.Highlight(false);
            hitObject = null;
        }

        grabbable = false;  // Ensure it resets when no item is detected
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
