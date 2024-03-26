using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed = 5f;

    public Transform combatLookAt;

    private Transform cameraTransform;


    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject aimCam;
    public GameObject topDownCam;

    public bool aim;
    [SerializeField]
    private int priorityBoostAmount = 10;
    [SerializeField]
    private Canvas thirdPersonCanvas;
    [SerializeField]
    private Canvas aimCanvas;


    [SerializeField] private CinemachineVirtualCamera combatVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;


    public CameraStyle currentStyle;
    public enum CameraStyle 
    {
        Basic,
        Combat,
        Aim,
        Topdown
    }

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransform = Camera.main.transform;

    }

    private void Update()
    {
        Debug.Log(currentStyle);
        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCameraStyle(CameraStyle.Basic);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCameraStyle(CameraStyle.Combat);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCameraStyle(CameraStyle.Topdown);
        }


        if (Input.GetMouseButtonDown(1))
        {

            StartAim();
            SwitchCameraStyle(CameraStyle.Aim);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            CancelAim();
            SwitchCameraStyle(CameraStyle.Combat);
        }

        /*// Check if right mouse button is held down and current style is combat
        if (Input.GetMouseButton(1) && currentStyle == CameraStyle.Combat)
        {
            SwitchCameraStyle(CameraStyle.Aim);
        }
        // Switch back to combat style if right mouse button is not held down and current style is aim
        else if (!Input.GetMouseButton(1) && currentStyle == CameraStyle.Aim)
        {
            SwitchCameraStyle(CameraStyle.Combat);
        }*/


        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        if(currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
                
            }
        }

        else if(currentStyle == CameraStyle.Combat)
        {


            // Get the rotation of the combatVirtualCamera
            Quaternion targetRotation = Quaternion.Euler(0f, combatVirtualCamera.transform.eulerAngles.y, 0f);

            //Quaternion targetRotation = Quaternion.Euler(0f, orientation.eulerAngles.y + 20f, 0f);

            // Interpolate towards the target rotation for the player
            playerObj.rotation = Quaternion.Lerp(playerObj.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Set orientation rotation to match player's rotation
            orientation.rotation = playerObj.rotation;

            // Update orientation position to follow the player
            orientation.position = player.position;


        }

        else if ( currentStyle == CameraStyle.Aim)
        {


            // Get the rotation of the combatVirtualCamera
            Quaternion targetRotation = Quaternion.Euler(0f, aimVirtualCamera.transform.eulerAngles.y, 0f);

            //Quaternion targetRotation = Quaternion.Euler(0f, orientation.eulerAngles.y + 20f, 0f);

            // Interpolate towards the target rotation for the player
            playerObj.rotation = Quaternion.Lerp(playerObj.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Set orientation rotation to match player's rotation
            orientation.rotation = playerObj.rotation;

            // Update orientation position to follow the player
            orientation.position = player.position;


        }


    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);



        combatVirtualCamera.gameObject.SetActive(true);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) 
        { 
            combatCam.SetActive(true); 
            //combatVirtualCamera.gameObject.SetActive(true);
        }
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;


    }

    private void StartAim()
    {
        aimVirtualCamera.Priority += priorityBoostAmount;
        aimCanvas.enabled = true;
        thirdPersonCanvas.enabled = false;
        combatCam.SetActive(false);
        aimCam.SetActive(true);
        
    }

    private void CancelAim()
    {
        aimVirtualCamera.Priority -= priorityBoostAmount;
        aimCanvas.enabled = false;
        thirdPersonCanvas.enabled = true;
        aimCam.SetActive(false);

    }


}