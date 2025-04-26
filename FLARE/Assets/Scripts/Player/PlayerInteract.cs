using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{

    public Camera cam;
    [SerializeField]
    private float distance = 8f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;

    public PlayerMovementAdvanced movement;




    //private PlayerInputManager inputManager;

    [Header("Keybinds")]
    public KeyCode interactKey = KeyCode.E;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();

        movement = GetComponentInParent<PlayerMovementAdvanced>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (movement != null && movement.isAiming1)
        {
            playerUI.UpdateText(string.Empty);
            return; // don't interact while aiming
        }
            
          

        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                playerUI.UpdateText(interactable.promptMessage);
                if (Input.GetKeyDown(interactKey))
                {
                    interactable.BaseInteract();
                }
                return;
            }
        }
        playerUI.UpdateText(string.Empty);
    }
}
