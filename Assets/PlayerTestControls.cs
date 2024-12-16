using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestControls : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Move left and right using A/D keys
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 move = transform.right * horizontalInput;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Jump using Space key
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }

        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
