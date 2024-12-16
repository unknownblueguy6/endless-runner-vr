using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float sideSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float tiltThreshold = 15f;
    public float maxSidePosition = 3f;

    private CharacterController characterController;
    private Camera mainCamera;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        moveDirection = Vector3.forward;
    }

    void Update()
    {
        // Ground Check
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Forward Movement
        Vector3 forwardMove = moveDirection * forwardSpeed * Time.deltaTime;

        // Side Movement based on head tilt
        float headTilt = mainCamera.transform.eulerAngles.z;
        if (headTilt > 180) headTilt -= 360;

        float sideMovement = 0f;
        if (Mathf.Abs(headTilt) > tiltThreshold)
        {
            sideMovement = headTilt > tiltThreshold ? -sideSpeed : sideSpeed;
        }

        Vector3 sideMove = Vector3.right * sideMovement * Time.deltaTime;

        // Jump
        if (Input.GetButtonDown("Fire1") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine movements
        Vector3 finalMove = forwardMove + sideMove;

        // Clamp horizontal position
        Vector3 newPosition = transform.position + finalMove;
        newPosition.x = Mathf.Clamp(newPosition.x, -maxSidePosition, maxSidePosition);
        finalMove = newPosition - transform.position;

        // Apply movement
        characterController.Move(finalMove);
        characterController.Move(velocity * Time.deltaTime);
    }
}

