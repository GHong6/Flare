using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int maxStackedItems = 24;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    [SerializeField] private Gun gun;
    public bool AddItem(Item item)
    {

        if (item == null)
        {
            Debug.LogError("Trying to add a null item to inventory!");
            return false;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.item == item &&
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();

                gun.UpdateAmmoInventoryUI();

                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);

                gun.UpdateAmmoInventoryUI();

                return true;
            }
        }
        return false;
    }

    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        gun.UpdateAmmoInventoryUI();
    }

    public int GetItemCount(Item item)
    {
        int count = 0;

        foreach (var slot in inventorySlots)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item == item)
            {
                count += inventoryItem.count;
            }
        }

        return count;
    }

    public void RemoveItems(Item item, int amount)
    {
        foreach (var slot in inventorySlots)
        {
            // Get the InventoryItem in the current slot
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();

            // Check if the slot contains the item we want to remove
            if (inventoryItem != null && inventoryItem.item == item)
            {
                // If the current slot has enough items to fulfill the request
                if (inventoryItem.count >= amount)
                {
                    inventoryItem.count -= amount; // Deduct the amount
                    inventoryItem.RefreshCount(); // Update the UI

                    // If the count reaches zero, remove the InventoryItem GameObject
                    if (inventoryItem.count <= 0)
                    {
                        Destroy(inventoryItem.gameObject);
                    }

                    gun.UpdateAmmoInventoryUI();

                    return; // Exit the method after removing the required amount
                }
                else
                {
                    // If the current slot doesn't have enough items, deduct as many as possible
                    amount -= inventoryItem.count; // Decrease the remaining amount to deduct
                    Destroy(inventoryItem.gameObject); // Remove the empty InventoryItem GameObject
                }
            }
        }

        // If the method completes and bullets are not removed, log for debugging
        if (amount > 0)
        {
            Debug.LogError($"Not enough {item.name} in inventory to remove {amount} items!");
        }

        gun.UpdateAmmoInventoryUI();
    }
}
