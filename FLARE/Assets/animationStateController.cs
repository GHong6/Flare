using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    public float movementSmoothing = 0.1f;

    public float speedSmoothness = 10f;

    [Header("References")]
    public Transform playerModel; // Assign your character model here

    private Rigidbody rb;
    private Animator animator;
    private Transform cameraTransform;

    private float turnSmoothVelocity;
    private Vector3 inputDirection;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private bool isRunning;

    private bool isAiming = false;
    public bool IsAiming { get; private set; }


    private float speed;

    private float targetSpeed;


    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        Cursor.visible = false;

        IsAiming = Input.GetMouseButton(1);
        animator.SetBool("IsAiming", IsAiming);
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isAiming = Input.GetMouseButton(1);
        animator.SetBool("IsAiming", isAiming);

        float targetAnimSpeed = inputDirection.magnitude * (isRunning ? runSpeed : walkSpeed);
        speed = Mathf.Lerp(speed, targetAnimSpeed, Time.deltaTime * speedSmoothness);
        animator.SetFloat("Speed", speed);
        //Debug.Log("Speed: " + speed);
    }

    void FixedUpdate()
    {
        if (isAiming)
        {
            // Rotate toward camera
            Vector3 aimDirection = cameraTransform.forward;
            aimDirection.y = 0f;
            aimDirection.Normalize();

            float targetAngle = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 targetDirection = cameraTransform.forward * inputDirection.z + cameraTransform.right * inputDirection.x;
            targetDirection.y = 0f;
            targetDirection.Normalize();

            if (!isAiming)
            {
                float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            moveDirection = Vector3.Lerp(moveDirection, targetDirection, movementSmoothing);
            targetSpeed = isRunning ? runSpeed : walkSpeed;

            // Use raw targetSpeed for actual movement
            Vector3 targetVelocity = new Vector3(moveDirection.x * targetSpeed, rb.velocity.y, moveDirection.z * targetSpeed);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, movementSmoothing);
        }
        else
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(0f, rb.velocity.y, 0f), ref currentVelocity, movementSmoothing);
            targetSpeed = 0f;
        }

        // Now blend animation speed smoothly (0 to 2 or 6)
        float animTargetSpeed = inputDirection.magnitude * (isRunning ? runSpeed : walkSpeed);
        speed = Mathf.Lerp(speed, animTargetSpeed, Time.deltaTime * speedSmoothness);
        animator.SetFloat("Speed", speed);
    }
}
