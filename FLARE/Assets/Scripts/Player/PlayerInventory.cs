using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public GameObject inventoryUI; // Assign the UI Image GameObject in the Inspector
    public KeyCode toggleKey = KeyCode.E; // Key to toggle inventory
    public MonoBehaviour cameraController; // Reference to the camera control script (e.g., FPS Controller)
    public MonoBehaviour playerController; // Reference to the player movement script (if needed)
    public MonoBehaviour gunSway;
    public MonoBehaviour shoot; // Reference to the Shoot script on the player
    public Gun gunScript; // Reference to the Gun script to check reload state

    private bool isInventoryOpen = false;

    void Update()
    {
        // Check if the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            // Only toggle the inventory if the gun is not reloading
            if (gunScript == null || !gunScript.GunData.reloading)
            {
                ToggleInventory();
            }
            else
            {
                Debug.Log("Cannot open inventory while reloading!");
            }
        }
    }   

    private void ToggleInventory()
    {
        // Toggle the inventory UI's active state
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        // Manage cursor state and disable/enable controls
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true;                 // Show the cursor

            cameraController.enabled = false; // Disable camera look-around
            gunSway.enabled = false;
            shoot.enabled = false; // Disable the Shoot script

            if (playerController != null)
                playerController.enabled = false; // Disable player movement (optional)
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            Cursor.visible = false;                  // Hide the cursor

            cameraController.enabled = true; // Enable camera look-around
            gunSway.enabled = true;
            shoot.enabled = true; // Enable the Shoot script

            if (playerController != null)
                playerController.enabled = true; // Enable player movement (optional)
        }
    }
}
