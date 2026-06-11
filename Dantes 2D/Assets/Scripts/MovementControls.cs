using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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


//Bool tracking if the player is touching the floor and can jump again
    private bool grounded;
//Bool tracking if the player is touching one of their sides on a wall
    private bool isTouchingWall;
    //private bool isWallSliding = false;
//Bool tracking the crouching state of the player
    private bool isCrouching;
//Bool tracking if the player is on a moving platform or elevator
    private bool isOnMovingGround;
//Bool tracking if the player is on a platform
    private bool isOnPlatform;

    [HideInInspector] public bool isLedge;
//Bool tracking if the player's jump is totally disabled (like after they just jumped)
    private bool isJumpDisable;


/// <summary>
/// SELF VARIABLES
/// </summary>

//The Player Object
    private Rigidbody2D player;
//The Player's Sprite
    private SpriteRenderer spriteRenderer;
//Player's collider
    private BoxCollider2D playerCollider;


    #endregion

    #region Awake, Start, Update
    // Awake is called at game initialization
    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        
        GroundMovement(moveInput);
        

    }

    private void FixedUpdate()
    {
        CheckGrounded();
        //CheckWallSliding();   
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
        GravityScaler();
        //Crouching
        ToggleCrouch();
        Drop();
        //Wallsliding
        HandleWallSliding(moveInput);
    }

    private void Strafing(float moveInput)
    {
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
            && (grounded || jumpCoyoteCounter > 0)
            && jumpCooldownCounter <= 0)
        {
                //disabled tests
                //if (grounded) {print("Jumped from Grounded");}
                //else if (!grounded && jCoyoteCounter > 0) {print("Jumped from Coyote Time");}

                player.linearVelocity = new Vector2(player.linearVelocity.x, jumpPressSpeed);
                jumpCoyoteCounter = 0;
                jumpBufferCounter = 0;
                jumpCooldownCounter = jumpCooldown;
                isJumpDisable = false;
                grounded = false;
        }
        else if (Input.GetKey(KeyCode.Space) && player.linearVelocityY>0 && !isJumpDisable)
        {
            player.AddForceY(jumpHoldSpeed);

        }
        else{isJumpDisable = true;}
    
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

                grounded = true;
            }
            else if (Ground.gameObject.tag == "Platform" && groundCheck.position.y > Ground.transform.position.y)
            {
                print("platform");
                isOnMovingGround = false;
                isOnPlatform = true;
                transform.SetParent(null, true);

                grounded = true;
            }
            else
            {
                isOnMovingGround = false;
                isOnPlatform = false;
                transform.SetParent(null, true);

                grounded = true;
            }
        }
        else if (Ground == null)
        { grounded = false; }

    }


//currently only seems to detect hitting the right of a wall
    private void CheckWallSliding()
    {isTouchingWall = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left: Vector2.right, wallCheckDistance, groundLayer);
        if (isTouchingWall == true)
        { print("touchingwall"); }
    }

 
    private void HandleWallSliding(float moveInput)
    {
        if (isTouchingWall == true && !grounded && moveInput == 0 && player.linearVelocityY <= 0)
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
      if (grounded)
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
        if (player.linearVelocityY > 0)
        {
            player.gravityScale = gravScaleUp;
        }
        else player.gravityScale = gravScaleDown;
    }

    //Interact Button
    private void Drop()
    {
        if (isCrouching == true && isOnPlatform && grounded && Input.GetKeyDown(KeyCode.Space)&& playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(crouchDisableTime));
        }
    }

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
