using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    public int selectedSlot = -1;

    private float scroll;
    [SerializeField] private ItemOnHand_Controller itemOnHand;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        ChangeSelectedSlot(0);
        itemOnHand = ItemOnHand_Controller.Instance;
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Unselect();
        }

        if (newValue > 4)
        {
            newValue = 0;
        }
        else if (newValue < 0)
        {
            newValue = 4;
        }

        selectedSlot = newValue;
        inventorySlots[selectedSlot].Select();

        // Ensure only the correct object is equipped
        itemOnHand.SwapItem(selectedSlot);
    }

    public GameObject GetStoredItem(int slotIndex)
    {
        return inventorySlots[slotIndex].storedGameObject;
    }

    public bool AddObject(InventoryObject invObject, GameObject realObj)
    {
        // 1. Check if an existing stackable item exists
        foreach (var slot in inventorySlots)
        {
            InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (objInSlot != null && objInSlot.inventoryObject == invObject && objInSlot.count < 5 && invObject.stackable)
            {
                objInSlot.count++;
                objInSlot.RefreshCount();
                //Destroy(realObj); // Destroy the duplicate GameObject (since it's stacked)
                return true;
            }
        }

        // 2. If no stackable slot found, place in an empty slot
        foreach (var slot in inventorySlots)
        {
            if (slot.storedGameObject == null)
            {
                slot.storedGameObject = realObj;
                realObj.SetActive(false); // Hide the object when stored

                // Spawn a new UI item for the inventory
                SpawnNewObject(invObject, slot);
                return true;
            }
        }

        return false; // Inventory full
    }


    void SpawnNewObject(InventoryObject obj, InventorySlot slot)
    {
        GameObject newObj = Instantiate(inventoryItemPrefab, slot.transform); // Spawn UI icon inside the slot
        InventoryItem inventoryItem = newObj.GetComponent<InventoryItem>();

        inventoryItem.InitializeItem(obj); // Assign the correct item data
    }


    public void RemoveObjectFromInventory(GameObject item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.storedGameObject == item)
            {
                slot.storedGameObject = null; // Remove reference

                // Remove UI representation
                InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (objInSlot != null)
                {
                    Destroy(objInSlot.gameObject); // Destroy the UI icon
                }

                Debug.Log("Item removed from inventory.");
                return;
            }
        }
    }

    public void OnToolbarSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scroll = context.ReadValue<float>();

            if (scroll > 0) // Scrolling up
            {
                ChangeSelectedSlot(selectedSlot - 1);
            }
            else if (scroll < 0) // Scrolling down
            {
                ChangeSelectedSlot(selectedSlot + 1);
            }
        }
    }


}
