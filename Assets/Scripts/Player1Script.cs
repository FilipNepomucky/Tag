using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player1Script : MonoBehaviour
{
    [Header("This Player")]
    [SerializeField] private Rigidbody2D player1;
    [SerializeField] public GameObject playerCollider;
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float movementSpeed = 40f;
    private float horizontalMovement = 0f;
    private bool canJump = false;
    private bool canWallJump = false;
    [SerializeField] public GameObject chaserIndicator;
    public bool isChaser = true;

    [Header("Opponent")]
    [SerializeField] private Player2Script player2;


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
            horizontalMovement = Input.GetAxisRaw("Horizontal1") * movementSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));

            if (Input.GetButtonDown("Jump1"))
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

    // Check for collision with Player 2
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isChaser && player2.isChaser == false && player2.timeStamp <= Time.time && collision.gameObject.CompareTag("Player2"))
        {
            peckSound.Play();
            MakePlayer2Chaser();
        }
    }

    // MY FUNCTIONS
    public void MakePlayer2Chaser()
    {
        // First remove chaser realted things from Player 1
        isChaser = false;
        chaserIndicator.SetActive(false);

        // Now  make Player 2 a chaser
        player2.isChaser = true;
        player2.chaserIndicator.SetActive(true);
        Debug.Log("Player 2 is now a chaser.");

        // Start the cooldown
        timeStamp = Time.time + cooldownLength;
    }

    public void OnLanding()
    {
        controller.isJumping = false;
        animator.SetBool("isJumping", false);
    }
}
