using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;  // Player to follow
    public Transform cameraTransform;  // Reference to the actual camera
    public float rotationSpeed = 3.0f;
    public float minYAngle = -20f;
    public float maxYAngle = 60f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        if (target == null || cameraTransform == null)
        {
            Debug.LogError("CameraOrbit: Target or Camera Transform is missing!");
            return;
        }

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Get mouse input
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp vertical rotation
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        // Rotate the Camera Pivot (NOT the Camera)
        transform.position = target.position;  // Keep pivot at player’s position
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
