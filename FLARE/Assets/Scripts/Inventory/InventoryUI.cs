using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Reference to the inventory panel
    [SerializeField] private Gun gun; // Reference to the Gun script

    private bool isInventoryOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Replace "I" with your inventory toggle key
        {
            if (gun != null && gun.IsReloading)
            {
                Debug.Log("Cannot open inventory while reloading!");
                return;
            }

            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
    }
}
