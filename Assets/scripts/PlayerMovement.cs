using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // Movement speed
    public float tileSize = 1f;       // The size of one tile (assuming uniform grid)
    public Rigidbody2D rb;            // 2D Rigidbody for movement
    public Animator animator;         // Animator component for handling animations

    private Vector2 targetPosition;   // The position the player will move to
    private bool isMoving = false;    // Check if the player is currently moving
    private Vector2 lastMoveDirection; // To store the last movement direction (for idle animations)

    void Start()
    {
        targetPosition = rb.position; // Initialize the target position as the current position
    }

    void Update()
    {
        if (!isMoving)  // Only allow input if the player is not already moving
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Prevent diagonal movement by prioritizing horizontal over vertical
            if (input.x != 0)
            {
                input.y = 0;  // Ignore vertical input if there is horizontal input
            }

            if (input != Vector2.zero)
            {
                // Set the target position one tile away in the chosen direction
                targetPosition = rb.position + input * tileSize;
                isMoving = true;  // Start the movement

                // Set animation parameters
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                animator.SetBool("isMoving", true);

                // Store the last movement direction for idle animations
                lastMoveDirection = input;
            }
            else
            {
                // If no input, set idle animation for the last direction
                animator.SetBool("isMoving", false);
                animator.SetFloat("moveX", lastMoveDirection.x);
                animator.SetFloat("moveY", lastMoveDirection.y);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // Move towards the target position
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // If the player reaches the target position, stop moving
            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition);  // Snap to the exact target position
                isMoving = false;  // Stop moving, allowing new input
            }
        }
    }
}
