﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Aim & Shoot")]
    public Transform orientation;
    public Transform playerObj;
    public Transform cameraTransform;

    [SerializeField]
    private GameObject bulletPrefab;
    //[SerializeField]
    //private Transform barrelTransform;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletRange = 50f;

    private Animator animator;

    float horizontalInput;
    float verticalInput;

    public bool isAiming1;

    private bool isShooting = false;
    public bool IsShooting
    {
        get { return isShooting; }
        private set { isShooting = value; }
    }

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        standing,
        walking,
        sprinting,
        crouching,
        sliding,
        air,
        aiming
    }

    public bool sliding;
    private PlayerHealth playerHealth;



    private bool canSprint = true;
    //private float stamina;
    private void Start()
    {
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();


        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        //stamina = playerHealth.maxStamina;

        startYScale = transform.localScale.y;
    }



    //private void ShootGun()
    //{
    //    RaycastHit hit;
    //    GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
    //    BulletController bulletController = bullet.GetComponent<BulletController>();
    //    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
    //    {
            
    //        bulletController.target = hit.point;
    //        bulletController.hit = true;
    //    }
    //    else
    //    {
   
    //        bulletController.target = cameraTransform.position + cameraTransform.forward * bulletRange;
    //        bulletController.hit = false;
    //    }

    //}


    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        //Debug.Log($"State: {state}, moveSpeed: {moveSpeed}, stamina: {playerHealth.GetStamina()}");

        MyInput();
        SpeedControl();
        StateHandler();
        //RotatePlayerObj();
        //UpdateOrientationDirection();

        HandleAnimations();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }


        //SHOOT
        if (Input.GetMouseButtonDown(0))
        {
            //ShootGun();
        }
    }

    private void StateHandler()
    {
        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;
            desiredMoveSpeed = OnSlope() && rb.velocity.y < 0.1f ? slideSpeed : sprintSpeed;
        }
        else if (grounded && Input.GetMouseButton(1) && horizontalInput == 0 && verticalInput == 0)
        {
            state = MovementState.aiming;
            desiredMoveSpeed = 0f;
            isAiming1 = true;
        }
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        //RUNNING
        else if (grounded && Input.GetKey(sprintKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            if (playerHealth.GetStamina() > 0 && !playerHealth.IsStaminaDepleted())
            {
                state = MovementState.sprinting;
                desiredMoveSpeed = sprintSpeed;
                playerHealth.ConsumeStamina();
            }
            else
            {
                // Either stamina is 0 and depleted, or stamina is 0 but not yet regenerated
                state = MovementState.walking;
                desiredMoveSpeed = walkSpeed;
            }
        }
        // Walking - if stamina is 0 or not sprinting
        else if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            playerHealth.StartStaminaRegen();

            if (playerHealth.GetStamina() >= playerHealth.maxStamina)
                canSprint = true;
        }
        else if (grounded)
        {
            state = MovementState.standing;
            desiredMoveSpeed = 0f;
            playerHealth.StartStaminaRegen();

            if (playerHealth.GetStamina() >= playerHealth.maxStamina)
                canSprint = true;
        }
        else
        {
            state = MovementState.air;
        }

        if (Input.GetMouseButton(1))
        {
            isAiming1 = true;
        }

        if (!Input.GetMouseButton(1))
        {
            isAiming1 = false;
        }

        // check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

    }


    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                //time += Time.deltaTime * speedIncreaseMultiplier;
                time += Time.deltaTime * speedIncreaseMultiplier * 10f;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //moveDirection1 = playerObj.forward * verticalInput + playerObj.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void UpdateOrientationDirection()
    {
        if (state != MovementState.standing)
        {
            Vector3 viewDir = cameraTransform.forward;
            viewDir.y = 0;
            orientation.forward = viewDir.normalized;
        }
    }
    private void RotatePlayerObj()
    {
        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.aiming)
        {
            Vector3 lookDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            playerObj.rotation = Quaternion.Slerp(playerObj.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void HandleAnimations()
    {
        if (!animator) return;

        float inputMagnitude = new Vector2(horizontalInput, verticalInput).magnitude;
        float animSpeed = inputMagnitude * moveSpeed;

        animator.SetFloat("Speed", animSpeed);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("IsCrouching", state == MovementState.crouching);
        animator.SetBool("IsSliding", state == MovementState.sliding);
        animator.SetBool("IsAiming", state == MovementState.aiming);
    }


    private IEnumerator Shoot()
    {
        IsShooting = true;
        yield return new WaitForSeconds(1f); // Shooting lasts 1 second
        IsShooting = false;
    }


}