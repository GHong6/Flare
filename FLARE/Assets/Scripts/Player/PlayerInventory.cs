using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public GameObject inventoryUI;
    public KeyCode toggleKey = KeyCode.Tab;

    [Header("References")]
    public ThirdPersonCam cameraController; // Drag your ThirdPersonCam script here
    public MonoBehaviour gunSway;
    public MonoBehaviour shoot;
    public Gun gunScript;

    public bool allowMouseLook = true;

    private bool isInventoryOpen = false;


    void Update()
    {
        // Toggle inventory with key press
        if (Input.GetKeyDown(toggleKey))
        {
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
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Disable camera rotation
            if (cameraController is ThirdPersonCam cam)
            {
                cam.allowRotation = false;
                cam.inventoryOpen = true;
                cam.DisableCameraInput(); // Optional if using CinemachineInputProvider
            }

            if (gunSway != null)
                gunSway.enabled = false;

            if (shoot != null)
                shoot.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Enable camera rotation
            if (cameraController is ThirdPersonCam cam)
            {
                cam.allowRotation = true;
                cam.inventoryOpen = false;
                cam.EnableCameraInput(); // Optional if using CinemachineInputProvider
            }

            if (gunSway != null)
                gunSway.enabled = true;

            if (shoot != null)
                shoot.enabled = true;
        }
    }
}
