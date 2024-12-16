using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    public Transform cameraTransform;
    private Vector3 offset;

    void Start()
    {
        // Get the Main Camera from XR Rig
        cameraTransform = Camera.main.transform;
        // Calculate initial offset between head and camera
        offset = transform.position - cameraTransform.position;
    }

    void LateUpdate()
    {
        // Update only the position, maintaining the head's place in player hierarchy
        transform.position = cameraTransform.position + offset;
        // Optional: Follow rotation as well
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
    }
}

