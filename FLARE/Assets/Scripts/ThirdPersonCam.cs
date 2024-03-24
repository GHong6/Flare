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
        }
        else if (Input.GetMouseButtonUp(1))
        {
            CancelAim();
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

        else if(currentStyle == CameraStyle.Combat || currentStyle == CameraStyle.Aim)
        {

            Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            playerObj.rotation = Quaternion.Lerp(playerObj.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            /*Vector3 dirtoCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirtoCombatLookAt.normalized;

            playerObj.forward = dirtoCombatLookAt.normalized;*/
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