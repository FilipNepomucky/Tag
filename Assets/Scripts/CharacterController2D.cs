using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float jumpForce = 700f;                          // Amount of force added when the player jumps.-
    [Range(0, .3f)][SerializeField] private float movementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool airControl = true;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask whatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.

    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool isGrounded { get; private set; }            // Whether or not the player is grounded.
    public bool isJumping = false;
    private Rigidbody2D rb;
    private bool isFacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 v3Velocity = Vector3.zero;

    [Header("Wall Jump")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask WhatIsWall;                          // A mask determining what is ground to the character
    private bool isTouchingWall;
    const float touchingWallRadius = .2f; // Radius of the overlap circle to determine if the player is touching a wall
    public bool isWallSliding { get; private set; }
    [SerializeField] private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f; // Wall jumping buffer
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Coyote Time")]
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;


    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);

        // Coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            isJumping = true;
        }

        // Now the same with wall
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, touchingWallRadius, WhatIsWall);
    }

    public void Move(float detectedHorizontalMovement, bool canJump, bool canWallJump)
    {
        // Only move the player horizontaly if he's grounded or airControl is turned on AND is not wall jumping
        if ((isGrounded || airControl) && !isWallJumping)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(detectedHorizontalMovement * 10f, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref v3Velocity, movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (detectedHorizontalMovement > 0 && !isFacingRight)
            {
                // ...flip the player...
                Flip();
            }
            // ...otherwise if the input is moving the player left and the player is facing right...
            else if (detectedHorizontalMovement < 0 && isFacingRight)
            {
                // ...flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if (coyoteTimeCounter > 0f && canJump)
        {
            // ...add a vertical force to the player.
            rb.AddForce(new Vector2(0f, jumpForce));
            isGrounded = false;
            isJumping = true;
            coyoteTimeCounter = 0f;
        }


        // If the player should slide a wall...
        if (isTouchingWall && !isGrounded && detectedHorizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }

        // If the player should wall jump...
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x; // To (wall)jump in the opposite direction that the player is now wall sliding
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        //if (canWallJump && wallJumpingCounter > 0 && !isGrounded) // Why this doesn't work?
        if (canWallJump && wallJumpingCounter > 0 && !Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround))
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            Flip();

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
}