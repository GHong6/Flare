using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Add this for TextMeshPro

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{


    [Header("UI")]
    public Image image; // The UI image of the draggable item
    public TextMeshProUGUI text;
    public TextMeshProUGUI countText; // Use TextMeshProUGUI for text

    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag; // The parent to return to after dragging



    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
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
        Debug.Log("Begin drag called");
        parentAfterDrag = transform.parent; // Save the current parent
        transform.SetParent(transform.root); // Temporarily reparent to the root
        transform.SetAsLastSibling(); // Ensure it's rendered on top
        image.raycastTarget = false; // Disable raycast to avoid blocking drops
        text.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging called");
        transform.position = Input.mousePosition; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag called");
        if (parentAfterDrag != null) // Ensure a valid parent exists
        {
            transform.SetParent(parentAfterDrag); // Set the new parent
        }
        image.raycastTarget = true; // Re-enable raycast for future interactions
        text.raycastTarget = true;
    }
}
