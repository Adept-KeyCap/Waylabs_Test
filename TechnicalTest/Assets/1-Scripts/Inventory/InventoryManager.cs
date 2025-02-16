using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton

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

        ChangeSelectedSlot(0); // Starter Slot
        itemOnHand = ItemOnHand_Controller.Instance;
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Unselect();
        }

        // If the newValue tries to go below or above the limit, selects the slot from the other end
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

    public bool AddObject(InventoryObject invObject, GameObject realObj) // When picking up an Item, check if there any space and assign it to the first empty slot found
    {
        // 1. Check if an existing stackable item exists
        foreach (var slot in inventorySlots)
        {
            InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (objInSlot != null && objInSlot.inventoryObject == invObject && objInSlot.count < 5 && invObject.stackable)
            {
                objInSlot.count++;
                objInSlot.RefreshCount();
                realObj.SetActive(false); // Hide the object when stored

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

                // New UI item for the inventory
                SpawnNewObject(invObject, slot);
                return true;
            }
        }

        return false; // Inventory full
    }

    //Create a New UI Icon GameObject inside the selected InventroySlot
    void SpawnNewObject(InventoryObject obj, InventorySlot slot)
    {
        GameObject newObj = Instantiate(inventoryItemPrefab, slot.transform);
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

                return;
            }
        }
    }

    #region - New Imput System -

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

    #endregion
}
