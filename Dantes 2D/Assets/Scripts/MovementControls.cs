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


  
//HSpeed: speed for moving horizontally
    [SerializeField] private float hSpeed;
//VPress: amount of force initially given when jumping
    [SerializeField] private float vPressSpeed;
//VHold: amount of force continually given while holding space
    [SerializeField] private float vHoldSpeed;
//WSpeed: downward speed while wallsliding
    [SerializeField] private float wSpeed;
//CSpeed: unused variable I think for when crouching?
    [SerializeField] private float cSpeed;
//Gravity applied when moving up
    [SerializeField] private float gravScaleUp;
//Gravity applied when moving down
    [SerializeField] private float gravScaleDown;

//Amount of seconds the player has to hit space early and still jump when hitting ground
    [SerializeField] private float jBufferTime = 0.15f;
//Amount of seconds the player has to hit space after leaving a platform where they can still jump
     [SerializeField] private float jCoyoteTime = 0.2f;

      [SerializeField] private float disableTime;

//Radius the player checks from groundCheck to determine if there is ground beneath the player
    public float groundCheckRadius = 0.2f;

//Radius the player checks to see if there are walls around them
    public float wallCheckDistance = 1f;

//The point childed to the Player Object that is used as the origin for ground checking
    public Transform groundCheck;

//The layer where all Ground Objects reside
     public LayerMask groundLayer;
//The layer for Moving Platforms and Elevators so that the game childs the player to them to inherit momentum
     public LayerMask movingGroundLayer;
//The Player Object
    private Rigidbody2D player;
//The Player's Sprite
    private SpriteRenderer spriteRenderer;

    private BoxCollider2D playerCollider;

//Counter variable for Coyote Time
    private float jCoyoteCounter;

//Bool tracking if the player is touching the floor and can jump again
    private bool grounded;
//Bool tracking if the player is touching one of their sides on a wall
    private bool isTouchingWall;
    //private bool isWallSliding = false;
//Bool tracking the crouching state of the player
    private bool isCrouching;
//Bool tracking if the player is on a moving platform or elevator
    private bool isOnMovingGround;
    //
    private bool isOnPlatform;
//Counter for Jump Buffering    
    private float jBufferCounter;
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += new Vector3(0, -0.5f);
            transform.localScale = new Vector2(0.8f, 1f);
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
        transform.position += new Vector3(0, 0.5f);
        gameObject.transform.localScale = new Vector2(0.8f, 2f); isCrouching = false;
        }
    
        CoyoteTimer();
        JumpBufferer();
        GravityScaler();
        pMovement(moveInput);
         if (isTouchingWall == true)
        { print("touchingwall"); }
       HandleWallSliding(moveInput);
       Drop();

    }

    private void FixedUpdate()
    {
        CheckGrounded();
        //CheckWallSliding();   
    }

    #endregion

    #region Voids

    //Player Movement Method
    private void pMovement(float moveInput)
    {


    //Strafing
        if (!Input.GetKey(KeyCode.S))
        {
        player.linearVelocity = new Vector2(moveInput * hSpeed, player.linearVelocity.y);
        }

        //commented out different system for wallcheck and trying to fix a bug I found a better way to fix

        // if (moveInput == 1 && !Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, groundLayer))
        // {
        //      player.linearVelocity = new Vector2(moveInput * hSpeed, player.linearVelocity.y);
        //      isTouchingWall=true;
        // }
        //  else if (moveInput == -1 && !Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, groundLayer))
        // {
        //     player.linearVelocity = new Vector2(moveInput * hSpeed, player.linearVelocity.y);
        //     isTouchingWall =true;
        // }
        // else
        // { player.linearVelocity = new Vector2(moveInput * hSpeed, player.linearVelocity.y);
        //     isTouchingWall =false;
        // }

        //Jumping

        if (jBufferCounter > 0 && !Input.GetKey(KeyCode.S))
        {
            if (grounded || jCoyoteCounter > 0)
            {
                player.linearVelocity = new Vector2(player.linearVelocity.x, vPressSpeed);
                jCoyoteCounter = 0;
                jBufferCounter = 0;
                grounded = false;
            }
        }
                    if (Input.GetKey(KeyCode.Space) && player.linearVelocityY>0)
        {
            player.AddForceY(vHoldSpeed);

        }
    }

    private void CheckGrounded()
    {

        Collider2D Ground = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Ground != null)
        {
            grounded = true;

            if (Ground.gameObject.tag == "Moving")
            {
                print("movingground");
                isOnMovingGround = true;
                transform.SetParent(Ground.gameObject.transform, true);

            }
            else if (Ground.gameObject.tag == "Platform")
            {
                print("platform");
                isOnMovingGround = false;
                isOnPlatform = true;
                transform.SetParent(null, true);
            }
            else
            {
                isOnMovingGround = false;
                isOnPlatform = false;
                transform.SetParent(null, true);
            }



        }
        else if (Ground == null)
        { grounded = false; }
        
        

    //     grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); 

    // //Additional Check to deal with moving platforms properly
    //  if(Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, movingGroundLayer))
    //  {
    //     Collider2D Platform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, movingGroundLayer);
        
    //     transform.SetParent(Platform.gameObject.transform, true);
    //     }
    //     else if (grounded)
    //     {
    //     transform.SetParent(null, true);
    //     }

    }


//currently only seems to detect hitting the right of a wall
    private void CheckWallSliding()
    {isTouchingWall = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left: Vector2.right, wallCheckDistance, groundLayer);}

 
    private void HandleWallSliding(float moveInput)
    {
        if (isTouchingWall == true && !grounded && moveInput == 0 && player.linearVelocityY <= 0)
        {
            print("Wallsliding engaged");
           // isWallSliding = true;
            player.linearVelocityY = -wSpeed;
            
        }
        else
        {
            //isWallSliding = false;
        }
    }

    private void CoyoteTimer()
    {
      if (grounded)
        { jCoyoteCounter = jCoyoteTime; }
        else
        { jCoyoteCounter -= Time.deltaTime; }
    }

    private void JumpBufferer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { jBufferCounter = jBufferTime; }
        else
        { jBufferCounter -= Time.deltaTime; }
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
            StartCoroutine(DisablePlayerCollider(disableTime));
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
