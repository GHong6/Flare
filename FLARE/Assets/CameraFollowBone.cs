using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollowBone : MonoBehaviour
{
    [Header("References")]
    public Transform cameraBone; // Assign the CameraBone (from the rig)

    [Header("Offsets")]
    public Vector3 positionOffset; // Optional position offset
    public Vector3 rotationOffset; // Optional rotation offset

    private void LateUpdate()
    {
        // Follow the CameraBone's position and rotation with offset
        transform.position = cameraBone.position + cameraBone.TransformDirection(positionOffset);
        transform.rotation = cameraBone.rotation * Quaternion.Euler(rotationOffset);
    }
}
