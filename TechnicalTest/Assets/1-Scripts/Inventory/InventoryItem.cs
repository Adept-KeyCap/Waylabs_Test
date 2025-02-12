using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("UI")]
    public Image image;
    public TMP_Text countText;

    [HideInInspector] public InventoryObject inventoryObject;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    public void InitializeItem(InventoryObject newObject)
    {
        inventoryObject = newObject;
        image.sprite = newObject.image; // Assign the correct icon
        RefreshCount();
    }


    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        if (parentAfterDrag != transform.parent) // Item moved to a different slot
        {
            // Get old and new slots
            InventorySlot oldSlot = parentAfterDrag.GetComponent<InventorySlot>();
            InventorySlot newSlot = transform.parent.GetComponent<InventorySlot>();

            if (oldSlot != null && newSlot != null)
            {
                // Move the stored GameObject reference
                newSlot.storedGameObject = oldSlot.storedGameObject;
                oldSlot.storedGameObject = null;

                // Update UI icons
                oldSlot.UpdateIcon();
                newSlot.UpdateIcon();
            }
        }
        transform.SetParent(parentAfterDrag);
    }

}
