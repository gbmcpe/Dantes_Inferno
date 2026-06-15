using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using UnityEngine.UIElements;

public class MovementControls : MonoBehaviour
{

    #region Variables
  
/// <summary>
/// MOVING VARIABLES
/// </summary>
[Header("Movement")]

//walkSpeed: speed for moving horizontally
    [SerializeField] private float walkSpeed;

//wallslideSpeed: downward speed while wallsliding
    [SerializeField] private float wallslideSpeed;

//crouchSpeed: unused variable I think for when crouching?
    [SerializeField] private float crouchSpeed;

    //The amount of time the player passes through platforms for while crouching
    [SerializeField] private float crouchDisableTime;


/// <summary>
/// JUMPING VARIABLES 
/// </summary>
[Header("Jumping")]

//jumpPressSpeed: amount of force initially given when jumping
    [SerializeField] private float jumpPressSpeed;
//jumpHoldSpeed: amount of force continually given while holding space
    [SerializeField] private float jumpHoldSpeed;
//Gravity applied when moving up
    [SerializeField] private float gravScaleUp;
//Gravity applied when moving down
    [SerializeField] private float gravScaleDown;
//Amount of seconds the player has to hit space early and still jump when hitting ground
    [SerializeField] private float jumpBufferTime = 0.15f;
        //Counter for Jump Buffering    
        private float jumpBufferCounter;
//Amount of seconds the player has to hit space after leaving a platform where they can still jump
    [SerializeField] private float jumpCoyoteTime = 0.2f;
        //Counter variable for Coyote Time
        private float jumpCoyoteCounter;
//Amount of seconds the player has to wait before starting a new jump
    [SerializeField] private float jumpCooldown;
        //Counter variable for jump cooldown    
        private float jumpCooldownCounter;

/// <summary>
/// SENTINEL VARIABLES
/// </summary>
[Header("World")]

//Radius the player checks from groundCheck to determine if there is ground beneath the player
    public float groundCheckRadius = 0.2f;
//Radius the player checks to see if there are walls around them
    public float wallCheckDistance = 1f;
//The point childed to the Player Object that is used as the origin for ground checking
    public Transform groundCheck;
//The layer where all Ground Objects reside
    public LayerMask groundLayer;

    private float lastXPos;
//Bool tracking if the player is touching the floor and can jump again
    private bool isGrounded;
//Bool tracking if the player is touching one of their sides on a wall
    private bool isTouchingWall;
    //private bool isWallSliding = false;
//Bool tracking the crouching state of the player
    private bool isCrouching;
//Bool tracking if the player is on a moving platform or elevator
    private bool isOnMovingGround;
//Bool tracking if the player is on a platform
    private bool isOnPlatform;
    [HideInInspector] public bool isLedgeDetected;

    private int facing = -1;

//Bool tracking if the player's jump is totally disabled (like after they just jumped)
    private bool canJump;
    private bool canClimb = true;
    private bool isClimbing;


/// <summary>
/// SELF VARIABLES
/// </summary>

//The Player Object
    private Rigidbody2D player;
//The Player's Sprite

[SerializeField] GameObject spriteObject;
    private SpriteRenderer spriteRenderer;
//Player's collider
    private BoxCollider2D playerCollider;
    private Animator animator;

    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    private Vector2 climbStartPos;
    private Vector2 climbEndPos;


    #endregion

    #region Awake, Start, Update
    // Awake is called at game initialization
    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        animator = spriteObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {

        float moveInput = Input.GetAxis("Horizontal");
        GroundMovement(moveInput);
        lastXPos = transform.position.x;   

        
       
        //CheckWallSliding();   
    }

    private void FixedUpdate()
    {
        CheckGrounded();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    #endregion

    #region Voids

    //Player Movement Method
    private void GroundMovement(float moveInput)
    {
        //Running
        Strafing(moveInput);

        //Jumping
        CoyoteTimer();
        JumpBufferer();
        JumpCooldownTimer();
        Jump();

        CheckLedge();

        //TODO: Airtime Increase

        //GravityScaler();

        //Crouching
        ToggleCrouch();
        Drop();
        //Wallsliding
        HandleWallSliding(moveInput);

        //TODO: Fall Clamping


    }

    private void Strafing(float moveInput)
    {
        FlipCharacter(moveInput);

        //TODO: Add Acceleration
        //TODO: Add Deceleration
        //TODO: Add Top Speed
        
        if (moveInput != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        
        if (!Input.GetKey(KeyCode.S))
            {
                player.linearVelocity = new Vector2(moveInput * walkSpeed, player.linearVelocity.y);
            }    
    }

    private void Jump()
    {
        //Initial Press
        //If the jumpbuffer timer is over 
        //AND you are not holding s (dropdown)
        //AND are grounded OR the coyote timer hasn't expired 
        //you can jump
        if (jumpBufferCounter > 0 
            && !Input.GetKey(KeyCode.S)
            && (isGrounded || jumpCoyoteCounter > 0)
            && jumpCooldownCounter <= 0)
        {
                //disabled tests
                //if (grounded) {print("Jumped from Grounded");}
                //else if (!grounded && jCoyoteCounter > 0) {print("Jumped from Coyote Time");}
                player.gravityScale = gravScaleUp;
                player.linearVelocity = new Vector2(player.linearVelocity.x, jumpPressSpeed);
                jumpCoyoteCounter = 0;
                jumpBufferCounter = 0;
                jumpCooldownCounter = jumpCooldown;
                canJump = false;
                isGrounded = false;
        }
        else if (Input.GetKey(KeyCode.Space) && player.linearVelocityY>0 && !canJump)
        {
            player.AddForceY(jumpHoldSpeed);

        }
        else{canJump = true; player.gravityScale = gravScaleDown;}
    
    }

    private void ToggleCrouch()
    {        
        if (Input.GetKeyDown(KeyCode.S))
            {
                transform.position += new Vector3(0, -0.45f);
                transform.localScale = new Vector2(0.8f, 0.9f);
                isCrouching = true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
            transform.position += new Vector3(0, 0.45f);
            gameObject.transform.localScale = new Vector2(0.8f, 1.8f); isCrouching = false;
            }
    }

    private void HandleWallSliding(float moveInput)
    {
        if (isTouchingWall == true && !isGrounded && moveInput == 0 && player.linearVelocityY <= 0)
        {
            print("Wallsliding engaged");
           // isWallSliding = true;
            player.linearVelocityY = -wallslideSpeed;
            
        }
        else
        {
            //isWallSliding = false;
        }
    }

    private void CoyoteTimer()
    {
      if (isGrounded)
        { jumpCoyoteCounter = jumpCoyoteTime; }
        else
        { jumpCoyoteCounter -= Time.deltaTime; }
    }

    private void JumpCooldownTimer()
    {
        if(jumpCooldownCounter >0)
        {jumpCooldownCounter -= Time.deltaTime;}
    }

    private void JumpBufferer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { jumpBufferCounter = jumpBufferTime; }
        else
        { jumpBufferCounter -= Time.deltaTime; }
    }

    private void GravityScaler()
    {
        //changed it from >0 to >1  to attempt to stop 'hovering'
        if (player.linearVelocityY > 1)
        {
            player.gravityScale = gravScaleUp;
        }
        else player.gravityScale = gravScaleDown;
    }

    //Interact Button
    private void Drop()
    {
        if (isCrouching == true && isOnPlatform && isGrounded && Input.GetKeyDown(KeyCode.Space)&& playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(crouchDisableTime));
        }
    }

    private void FlipCharacter(float moveInput)
    {
        if ((moveInput > 0) && (transform.position.x > lastXPos)&&facing !=1)
        {//We are to the right, facing positive infinity
        transform.Rotate(0, 180f, 0);
        facing = 1;
        }
        else if ((moveInput <0) && (transform.position.x <= lastXPos)&&facing !=-1)
        {//We are to the left, facing negative infinity
        transform.Rotate(0, 180f, 0);
        facing = -1;
        }
    }
    
    #region Check Method Voids
    private void CheckGrounded()
    {

        Collider2D Ground = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Ground != null)
        {

            if (Ground.gameObject.tag == "Moving")
            {
                print("movingground");
                isOnMovingGround = true;
                transform.SetParent(Ground.gameObject.transform, true);

                isGrounded = true;
            }
            else if (Ground.gameObject.tag == "Platform" && groundCheck.position.y > Ground.transform.position.y)
            {
                print("platform");
                isOnMovingGround = false;
                isOnPlatform = true;
                transform.SetParent(null, true);

                isGrounded = true;
            }
            else
            {
                isOnMovingGround = false;
                isOnPlatform = false;
                transform.SetParent(null, true);

                isGrounded = true;
            }
        }
        else if (Ground == null)
        { isGrounded = false; }

    }

    private void CheckLedge()
    {
        if (isLedgeDetected && canClimb && Input.GetKeyDown(KeyCode.Space))
        {
            print ("Climb Start");
            canClimb = false;
            Vector2 ledgePos = GetComponentInChildren<LedgeDetector>().transform.position;

            climbStartPos = ledgePos+new Vector2(offset1.x*facing*-1, offset1.y);
            climbEndPos = ledgePos+new Vector2(offset2.x*facing*-1, offset2.y);

            isClimbing = true;
        }

        if(isClimbing)
        {
            print("Climb Lock");
        //1    
        transform.position = climbStartPos;

        player.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        //Temp way to call this, will change once we have an actual climb animation to sync with
        Invoke("LedgeClimbOver", 0.4f);
        }
    }
    private void LedgeClimbOver()
    {
    isClimbing =false;
    //2
    transform.position = climbEndPos;
    print("Climb End");

    player.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    player.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

    Invoke("AllowLedgeGrab", 0.25f);
    }

    private void AllowLedgeGrab() => canClimb=true;

    
//currently only seems to detect hitting the right of a wall
    private void CheckWallSliding()
    {isTouchingWall = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left: Vector2.right, wallCheckDistance, groundLayer);
        if (isTouchingWall == true)
        { print("touchingwall"); }
    }

    #endregion

    #endregion

    #region Value-Returning

    private IEnumerator DisablePlayerCollider(float disableT)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableT);
        playerCollider.enabled = true;
    }
    
    #endregion

    #region collisions / triggers

    //Old jumping check

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //if (collision.gameObject.tag == "Ground")
    // {
    //     grounded = true;
    // }
    //}

    private Vector2 SavePoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            SavePoint = transform.position;
            print("Save Point Reached");
        }
        else if (other.gameObject.tag == "Obstacle")
        {
            if (SavePoint != null)
                transform.position = SavePoint;
            else
            {
                transform.position = Vector3.zero;
            }
        }

    }

    #endregion

}
