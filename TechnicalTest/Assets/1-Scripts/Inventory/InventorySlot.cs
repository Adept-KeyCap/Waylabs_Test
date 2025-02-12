using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, unselectedColor;

    [HideInInspector] public GameObject storedGameObject; // The GameObject this slot holds

    private void Awake()
    {
        Unselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Unselect()
    {
        image.color = unselectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // Ensure slot is empty before assigning
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;

            // Get old and new slots
            InventorySlot oldSlot = inventoryItem.parentAfterDrag.GetComponent<InventorySlot>();
            InventorySlot newSlot = transform.GetComponent<InventorySlot>();

            if (oldSlot != null && newSlot != null)
            {
                //  Move the GameObject reference
                newSlot.storedGameObject = oldSlot.storedGameObject;
                oldSlot.storedGameObject = null;

                // Update UI
                oldSlot.UpdateIcon();
                newSlot.UpdateIcon();
            }
        }
    }


    public void UpdateIcon()
    {
        if (storedGameObject != null)
        {
            InventoryItem inventoryItem = storedGameObject.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                image.sprite = inventoryItem.inventoryObject.image;
                image.enabled = true;
            }
        }
        else
        {
            image.enabled = false; // Hide icon if slot is empty
        }
    }

}
