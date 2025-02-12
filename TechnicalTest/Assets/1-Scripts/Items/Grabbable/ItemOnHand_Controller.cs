using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemOnHand_Controller : MonoBehaviour
{
    public static ItemOnHand_Controller Instance;

    [Header("Item Detection")]
    public Item detected_Item;
    public Item held_Item;

    [Header("Grab Attributes")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabDistance = 3f;

    private Transform aimObj;
    private Transform playerCamera;
    private bool grabbable = false;
    private Item hitObject;
    private float pressStartTime = 0f;
    private bool isHolding = false;

    [SerializeField] private InventoryManager invManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        aimObj = CameraReferences.Instance.aimObject.transform;
        playerCamera = CameraReferences.Instance.playerCamera.transform;
    }

    void FixedUpdate()
    {
        Vector3 rayDirection = (aimObj.position - playerCamera.position).normalized;

        if (Physics.Raycast(playerCamera.position, rayDirection, out RaycastHit hit, grabDistance))
        {
            detected_Item = hit.collider.GetComponent<Item>();

            if (detected_Item != null)
            {
                if (hitObject != detected_Item)
                {
                    ResetHighlight();
                    hitObject = detected_Item;
                    hitObject.Highlight(true);
                }

                grabbable = true;
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
    }

    public void GrabItem()
    {
        if (grabbable && hitObject != null)
        {
            grabbable = false;
            hitObject.Grabbed(grabPoint); // Attach item to grab point

            // Store the item in inventory
            bool added = invManager.AddObject(hitObject.inventoryObject, hitObject.gameObject);

            if (!added)
            {
                Debug.Log("Inventory full! Cannot pick up item.");
                return;
            }

            //  If nothing is in hand, equip this item immediately
            if (held_Item == null)
            {
                EquipItem(hitObject.gameObject);
            }
            else
            {
                //  If an item is already held, just store it (disable the new item)
                hitObject.gameObject.SetActive(false);
            }
        }
    }

    private void EquipItem(GameObject item)
    {
        if (held_Item != null)
        {
            held_Item.gameObject.SetActive(false); // Hide current item if there is one
        }

        item.SetActive(true);
        held_Item = item.GetComponent<Item>();

        if (held_Item.GetComponent<Weapon>() != null)
        {
            held_Item.GetComponent<Weapon>().AmmoDisplay(true);
        }
    }

    public void SwapItem(int newItemId)
    {
        if (held_Item != null)
        {
            held_Item.gameObject.SetActive(false);
        }

        GameObject newItem = invManager.GetStoredItem(newItemId);

        if (newItem != null)
        {
            newItem.SetActive(true);
            held_Item = newItem.GetComponent<Item>();

            if (held_Item.GetComponent<Weapon>() != null)
            {
                held_Item.GetComponent<Weapon>().AmmoDisplay(true);
            }
        }
        else
        {
            held_Item = null;
        }
    }



    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabItem();
        }
    }

    public void OnHoldQ(InputAction.CallbackContext context)
    {
        if (context.started) // Start holding Q
        {
            pressStartTime = Time.time;
            isHolding = true;
        }
        else if (context.canceled) // Release Q to drop item
        {
            float holdDuration = Time.time - pressStartTime;
            isHolding = false;

            if (held_Item != null)
            {
                // Remove from InventoryManager (both storage and UI)
                invManager.RemoveObjectFromInventory(held_Item.gameObject);

                // Drop the item in world space
                held_Item.transform.SetParent(null); // Unparent from player
                held_Item.gameObject.SetActive(true); // Ensure it's visible

                // Throw it (optional)
                held_Item.ThrowItem(holdDuration);

                // Hand is now empty
                held_Item = null;
            }
        }
    }


    private void ResetHighlight()
    {
        if (hitObject != null)
        {
            hitObject.Highlight(false);
            hitObject = null;
        }

        grabbable = false;
    }


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
}
