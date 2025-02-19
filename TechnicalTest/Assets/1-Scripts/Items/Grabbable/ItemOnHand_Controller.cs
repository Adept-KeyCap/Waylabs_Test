﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemOnHand_Controller : MonoBehaviour
{
    public static ItemOnHand_Controller Instance;

    public AudioClip swapAudio; 
    public AudioClip throwAudio; 

    [Header("Item Detection")]
    public Item detected_Item;
    public Item held_Item;

    [Header("Grab Attributes")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabDistance = 3f;
    [SerializeField] private Slider throwSlider;

    private Transform aimObj;
    private Transform playerCamera;
    private AudioSource audioSource;
    private bool grabbable = false;
    private Item hitObject;
    private float pressStartTime = 0f;

    [SerializeField] private InventoryManager invManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        aimObj = CameraReferences.Instance.aimObject.transform;
        playerCamera = CameraReferences.Instance.playerCamera.transform;
        audioSource = GetComponent<AudioSource>();

        //Sets the throw force slider to 0 and disable it
        throwSlider.value = 0;
        throwSlider.gameObject.SetActive(false);
    }

    void FixedUpdate() // Constantly check for "Items"(Grabbable Objects) in the position of the crosshair
    {
        Vector3 rayDirection = (aimObj.position - playerCamera.position).normalized;

        if (Physics.Raycast(playerCamera.position, rayDirection, out RaycastHit hit, grabDistance))
        {
            detected_Item = hit.collider.GetComponent<Item>();

            if (detected_Item != null)
            {
                if (hitObject != detected_Item) // If the Item detected is not already stored as "hitObject", do it and highlight the GameObject
                {
                    ResetHighlight();
                    hitObject = detected_Item;
                    hitObject.Highlight(true);
                }

                grabbable = true; // the item is grabbable
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
        // Tell the item to attach to the player's hand when picking it up and add it to the inventory
        // Or activate the Item stored in the inventory to equip in hand
        if (grabbable && hitObject != null)
        {
            grabbable = false;
            hitObject.Grabbed(grabPoint); // Attach item to grab point

            
            bool added = invManager.AddObject(hitObject.inventoryObject, hitObject.gameObject); // Store the item in inventory

            if (!added)
            {
                // Inventory full, cannot pick up more items
                return;
            }

            
            if (held_Item == null) // If nothing is in hand, equip this item immediately
            {
                EquipItem(hitObject.gameObject);
            }
            else
            {
                hitObject.gameObject.SetActive(false); // If an item is already held, just store it (disable the new item)
            }
        }
    }

    private void EquipItem(GameObject item) //Set the item as the "held_Item"(The one we have at hand) and then check if it is a weapon
    {
        if (held_Item != null)
        {
            held_Item.gameObject.SetActive(false); // Hide current item if there is one
        }

        item.SetActive(true);
        held_Item = item.GetComponent<Item>();

        if (held_Item.GetComponent<Weapon>() != null) // if the Held item is a Weapon and it isn't null, display the bullet count
        {
            held_Item.GetComponent<Weapon>().AmmoDisplay(true);
        }
    }

    public void SwapItem(int newItemId) // Used when whe use the scroll wheel to navigate through the inventory
    {
        throwSlider.gameObject.SetActive(false);
        throwSlider.value = 0;
        StopCoroutine(FillThrowBar());

        // Player Feedback audio
        audioSource.clip = swapAudio;
        audioSource.Play();

        if (held_Item != null) // deactivate the current held item
        {
            held_Item.gameObject.SetActive(false);
        }

        GameObject newItem = invManager.GetStoredItem(newItemId); // Get the reference from "InventoryManager"

        if (newItem != null)
        {
            newItem.SetActive(true); // activate the GameObject associated with the InventorySlot and set is as the held_Item
            held_Item = newItem.GetComponent<Item>();

            if (held_Item.GetComponent<Weapon>() != null)
            {
                held_Item.GetComponent<Weapon>().AmmoDisplay(true);
            }
        }
        else
        {
            held_Item = null; // If there is nothing in the InventorySlot, there is not held_Item
        }
    }



    #region - New InputSystem -

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

            throwSlider.gameObject.SetActive(true);
            StartCoroutine(FillThrowBar());
        }
        else if (context.canceled) // Release Q to drop item
        {
            float holdDuration = Time.time - pressStartTime;

            if (held_Item != null)
            {
                // Remove from InventoryManager (both storage and UI)
                invManager.RemoveObjectFromInventory(held_Item.gameObject);

                // Drop the item in world space
                held_Item.transform.SetParent(null); // Unparent from player
                held_Item.gameObject.SetActive(true); // Ensure it's visible

                // Throw it 
                held_Item.ThrowItem(holdDuration * 2);
                throwSlider.gameObject.SetActive(false);
                StopCoroutine(FillThrowBar());

                audioSource.clip = throwAudio;
                audioSource.Play();

                // Hand is now empty
                held_Item = null;
            }
        }
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

    #endregion


    private void ResetHighlight()
    {
        if (hitObject != null)
        {
            hitObject.Highlight(false);
            hitObject = null;
        }

        grabbable = false;
    }

    private IEnumerator FillThrowBar() // Cycle through this to fill the throw force slider
    {
        while (true)
        {
            float holdDuration = Time.time - pressStartTime;
            throwSlider.value = holdDuration;

            yield return null;
        }

    }
}
