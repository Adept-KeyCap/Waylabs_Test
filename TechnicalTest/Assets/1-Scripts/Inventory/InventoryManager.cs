using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
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

        // Call SwapItem() when switching slots
        itemOnHand.SwapItem(selectedSlot);
    }


    public bool AddObject(InventoryObject invObject, GameObject realObj)
    {
        // Check if same item can be stacked
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (objInSlot != null && objInSlot.inventoryObject == invObject && objInSlot.count < 5 && objInSlot.inventoryObject.stackable)
            {
                objInSlot.count++;
                objInSlot.RefreshCount();
                return true;
            }
        }
        
        //Check for any empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(objInSlot == null)
            {
                SpawnNewObject(invObject, slot);
                return true;
            }
        }

        return false;
    }

    public InventoryObject GetSelectedObject(bool dropped)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem objInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (objInSlot != null)
        {
            InventoryObject obj = objInSlot.inventoryObject;
            if(dropped == true)
            {
                objInSlot.count--;
                if (objInSlot.count <= 0)
                {
                    Destroy(objInSlot);
                }
                else
                {
                    objInSlot.RefreshCount();
                }
            }

            return obj;
        }

        return null;
    }

    public void OnToolbarSelect(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            scroll = context.ReadValue<float>();
            if (scroll > 0 && selectedSlot >= 0 && selectedSlot <= 4)
            {
                ChangeSelectedSlot(selectedSlot - 1);
            }
            else if (scroll < 0 && selectedSlot >= 0 && selectedSlot <= 4)
            {
                ChangeSelectedSlot(selectedSlot + 1);
            }
        }     
    }

    void SpawnNewObject(InventoryObject obj, InventorySlot slot)
    {
        GameObject newObj = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newObj.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(obj); 
    }
}
