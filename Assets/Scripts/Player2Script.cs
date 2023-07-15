using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player2Script : MonoBehaviour
{
    [Header("This Player")]
    [SerializeField] private Rigidbody2D player2;
    [SerializeField] public GameObject playerCollider;
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float movementSpeed = 40f;
    private float horizontalMovement = 0f;
    private bool canJump = false;
    private bool canWallJump = false;
    [SerializeField] public GameObject chaserIndicator;
    public bool isChaser = true;

    [Header("Opponent")]
    [SerializeField] private Player1Script player1;


    [Header("Audio")]
    [SerializeField] private AudioSource peckSound;

    [Header("Other")]
    public Animator animator;
    private float cooldownLength = 0.1f;
    public float timeStamp;

    // Update is called once per frame
    void Update()
    {
        // MOVEMENT PART 1
        if (GameManagerScript.gamePlaying)
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal2") * movementSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));

            if (Input.GetButtonDown("Jump2"))
            {
                canJump = true;
                canWallJump = true;
            }

            // Animation
            if (controller.isJumping)
            {
                animator.SetBool("isJumping", true);
            }

            if (controller.isWallSliding)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isWallSliding", true);
            }
            else
            {
                animator.SetBool("isWallSliding", false);
            }
        }
    }

    // MOVEMENT PART 2
    private void FixedUpdate()
    {
        controller.Move(horizontalMovement * Time.fixedDeltaTime, canJump, canWallJump);
        canJump = false;
        canWallJump = false;
    }

    // CHECKING FOR COLLISION
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isChaser && player1.isChaser == false && player1.timeStamp <= Time.time && collision.gameObject.CompareTag("Player1"))
        {
            peckSound.Play();
            MakePlayer1Chaser();
        }
    }

    // MY FUNCTIONS
    public void MakePlayer1Chaser()
    {
        // First remove chaser realted things from Player 2
        isChaser = false;
        chaserIndicator.SetActive(false);

        // Now  make Player 1 a chaser
        player1.isChaser = true;
        player1.chaserIndicator.SetActive(true);
        Debug.Log("Player 1 is now a chaser.");

        // Start the cooldown
        timeStamp = Time.time + cooldownLength;
    }

    public void OnLanding()
    {
        controller.isJumping = false;
        animator.SetBool("isJumping", false);
    }
}
