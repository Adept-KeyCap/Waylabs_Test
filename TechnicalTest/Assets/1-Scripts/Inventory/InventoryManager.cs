using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    [SerializeField] private int selectedSlot = -1;

    private float scroll;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >=0)
        {
            inventorySlots[selectedSlot].Unselect();
        }

        if (newValue > 4 )
        {
            newValue = 0;
        }
        else if (newValue < 0)
        {
            newValue = 4;
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddObject(InventoryObject invObject)
    {
        // Check if same item can be stacked
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (objInSlot != null && objInSlot.inventoryObject == invObject && objInSlot.count < 5 && objInSlot.inventoryObject.stackable)
            {
                //Temporal
                objInSlot.count++;
                objInSlot.RefreshCount();
                return true;
            }
        }
        
        //Check for any empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryObject objInSlot = slot.GetComponentInChildren<InventoryObject>();
            if(objInSlot == null)
            {
                //Temporal
                SpawnNewObject(invObject, slot);
                return true;
            }
        }

        return false;
    }

    public void OnToolbarSelect(InputAction.CallbackContext context)
    {
        scroll = context.ReadValue<float>();
        if (scroll > 0)
        {
            ChangeSelectedSlot(selectedSlot-1);
        }
        else if (scroll < 0)
        {
            ChangeSelectedSlot(selectedSlot+1);
        }
    }

    void SpawnNewObject(InventoryObject obj, InventorySlot slot)
    {
        GameObject newObj = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newObj.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(obj); 
    }
}
