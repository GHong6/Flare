using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition; // Position of the camera in the rig
    //public Transform leftHand; // Reference to the left hand transform
    //public Transform rightHand; // Reference to the right hand transform

    private void Update()
    {
        // Follow the camera's position
        transform.position = cameraPosition.position;

        // Apply camera rotation to the hands
        //UpdateHandOrientation(leftHand);
        //UpdateHandOrientation(rightHand);
    }

    private void UpdateHandOrientation(Transform hand)
    {
        // Rotate the hands to match the camera's forward direction
        // If the hands are backwards, we need to apply a 180-degree rotation around their local Y-axis
        hand.rotation = cameraPosition.rotation * Quaternion.Euler(0f, 180f, 0f);
    }
}
