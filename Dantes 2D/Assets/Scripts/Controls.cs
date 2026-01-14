using System;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{

    #region Variables


  

    [SerializeField] private float hSpeed;
    [SerializeField] private float vPressSpeed;
    [SerializeField] private float vHoldSpeed;
    [SerializeField] private float wSpeed;
    [SerializeField] private float cSpeed;
    [SerializeField] private float gravScaleUp;
    [SerializeField] private float gravScaleDown;

    [SerializeField] private float jBufferTime = 0.15f;
     [SerializeField] private float jCoyoteTime = 0.2f;

    public float groundCheckRadius = 0.2f;

    public float wallCheckDistance = 1f;

    public Transform groundCheck;
     public LayerMask groundLayer;
    private Rigidbody2D player;
    private SpriteRenderer spriteRenderer;

    private float jCoyoteCounter;
    private bool grounded;
    private bool isTouchingWall;
    //private bool isWallSliding = false;
    private bool crouching;

    
    private float jBufferCounter;
    #endregion

    #region Awake, Start, Update
    // Awake is called at game initialization
    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += new Vector3(0, -0.25f, 0);
            transform.localScale = new Vector2(1, 0.5f);
            crouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
        transform.position += new Vector3(0, 0.25f, 0);
        gameObject.transform.localScale = new Vector3(1f, 1f, 0); crouching = false;
        }
    
        CoyoteTimer();
        JumpBufferer();
        GravityScaler();
        pMovement(moveInput);
         if (isTouchingWall == true)
        { print("touchingwall"); }
       HandleWallSliding(moveInput);

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
    { grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); }


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
    private void Interact()
    {

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
