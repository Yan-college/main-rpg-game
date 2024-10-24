using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;              // Movement speed
    public float tileSize = 1f;               // The size of one tile
    public float moveTimeout = 1f;            // Max time to reach a tile before moving back
    public Rigidbody2D rb;                    // 2D Rigidbody for movement
    public Animator animator;                 // Animator component for handling animations
    public LayerMask obstacleLayer;           // Layer mask for detecting obstacles

    private Vector2 targetPosition;           // Position the player will move to
    private Vector2 previousPosition;         // Previous tile position
    private bool isMoving = false;            // Check if the player is currently moving
    private Vector2 lastMoveDirection;        // To store the last movement direction (for idle animations)
    private float moveTimer;                  // Timer to track how long the player has been moving

    void Start()
    {
        targetPosition = rb.position;         // Initialize the target position as the current position
        previousPosition = rb.position;       // Store the initial position as the previous tile
        moveTimer = 0f;
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (!isMoving)  // Only allow input if the player is not already moving
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Prevent diagonal movement by prioritizing horizontal over vertical
            if (input.x != 0)
            {
                input.y = 0;  // Ignore vertical input if there is horizontal input
            }

            if (input != Vector2.zero)
            {
                // Set the previous position before starting the move
                previousPosition = rb.position;

                // Set the target position one tile away in the chosen direction
                targetPosition = rb.position + input * tileSize;
                isMoving = true;  // Start moving

                // Set animation parameters to match movement direction
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                animator.SetBool("isMoving", true);

                // Update the last move direction (for idle animations)
                lastMoveDirection = input;

                // Reset the movement timer
                moveTimer = 0f;
            }
            else
            {
                // If no input, set idle animation with the last move direction
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

            // Update the movement timer
            moveTimer += Time.fixedDeltaTime;

            // If the player reaches the target position, stop moving
            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition);  // Snap to the exact target position
                isMoving = false;  // Stop movement

                // Reset the movement timer
                moveTimer = 0f;
            }

            // If the player doesn't reach the target within the allowed time, move back to the previous tile
            if (moveTimer >= moveTimeout)
            {
                rb.MovePosition(previousPosition);  // Snap back to the previous tile
                isMoving = false;  // Stop movement
                animator.SetBool("isMoving", false);  // Set idle animation

                // Reset the timer and target position
                targetPosition = previousPosition;
                moveTimer = 0f;
            }
        }
    }
}
