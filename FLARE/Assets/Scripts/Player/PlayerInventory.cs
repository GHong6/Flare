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

    public bool allowMouseLook = true;

    private bool isInventoryOpen = false;


    void Update()
    {

        if (!allowMouseLook) return;

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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Freeze player input
            if (playerController != null)
                playerController.enabled = false;

            //Prevent mouse look
            //if (cameraController is MonoBehaviour cameraInputScript)
            //    cameraInputScript.enabled = false;

            if (cameraController is ThirdPersonCam camScript)
                camScript.allowRotation = false;


            gunSway.enabled = false;
            shoot.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerController != null)
                playerController.enabled = true;

            //if (cameraController is MonoBehaviour cameraInputScript)
            //    cameraInputScript.enabled = true;

            if (cameraController is ThirdPersonCam camScript)
                camScript.inventoryOpen = true;

            gunSway.enabled = true;
            shoot.enabled = true;
        }
    }
}
