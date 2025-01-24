using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    // Called when an item is dropped onto this slot
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // Ensure the slot is empty
        {
            GameObject dropped = eventData.pointerDrag; // Get the dragged object
            if (dropped != null)
            {
                InventoryItem draggableItem = dropped.GetComponent<InventoryItem>();
                if (draggableItem != null)
                {
                    draggableItem.parentAfterDrag = transform; // Update the new parent

                }
            }
        }
    }
}
