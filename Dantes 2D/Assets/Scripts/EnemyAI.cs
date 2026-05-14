using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //  Movement Section
    public bool CanMove;  // Check to determine if can move
    public float MoveSpeed;  // Max speed of movement
    public float ChaseSpeed;  // Speed when chasing the target
    private float CurrentSpeed;  //  Current speed
    private Rigidbody2D EnemyRB;  // Rigidbody2D component attached to the enemy
    private bool facingRight;  // Check to determine if facing right
    private bool isGrounded;  // Check to determine if grounded
    private bool HitWall;  // Check to determine if hitting a wall
    private bool HitWallR;  // Check to determine if hitting a wall
    private bool groundBelow;  // Check to determine if there is ground below
    
    //  Sprite Section

    //  Attack Section

        //  Shooting Sub-section
    public GameObject SummonedObject;  //  Bullet prefab to be instantiated when shooting
    public Transform SummonLocation;  //  Location where the object will be spawned
    public bool CanShoot;  // Check to determine if can melee
    public int FireDelay;  // Time between shots in seconds
    public int GunDistance; // Maximum distance starting to shoot
    public int BulletTotal; // Total number of bullets allowed at once
    private List<GameObject> activeObjects = new List<GameObject>();  // List to keep track of summoned objects
    public bool AimToShoot;  // Decides if enemy will stop to shoot
        
        //  Melee Sub-section
    public GameObject MeleeObject;  // Melee prefab to be instantiated when melee
    public Transform MeleeLocation;  // Location where the melee will be spawned
    public bool CanMelee;  // Check to determine if can melee
    public int MeleeDelay;  // Time between melee attacks in seconds
    public int MeleeDistance; // Maximum distance starting to melee
    //  HP System
    public int MaxHp;  // Maximum health
    private int currentHp; // Current health
    //  Other
    private float delay;  //  Delay timer for various features
    private Transform target;  // Reference to the player or target
    private bool playerdetected;  // Check to determine if player is detected
    public LayerMask groundLayer; // Put ground layer here
    public int detectionRange;  // Check to determine if target is in sight,


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;  // Find player, allows change to targets
        currentHp = MaxHp;  // Set current health to maximum health at the start
        EnemyRB = GetComponent<Rigidbody2D>();  // Gets the Rigidbody2D component attached to the enemy
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        //  HP System
        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
        
        //  Combat Check
        CombatCheck();
        
        //  Movement Check
        if (CanMove == true)    
    {   
        Movement();
    }

    void CombatCheck()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        //  Shooting Check
        if(distance < GunDistance && distance > MeleeDistance && CanShoot == true)
        {
            delay += Time.deltaTime;

             if(delay > FireDelay)  
            {
                delay = 0;
                Shoot();
            } 
        }
        //  Melee Check
        if (distance < MeleeDistance && CanMelee == true)
        {
            delay += Time.deltaTime;

             if(delay > MeleeDelay)
            {
                delay = 0;
                Melee();
            } 
        }
    }

    void Shoot()  //  Shoots, will add more info later
    {
        if (AimToShoot == true)
                {
                    CurrentSpeed = 0;  // Stop movement to shoot
                }
        activeObjects.RemoveAll(item => item == null);  // Remove null entries from the list
        if(activeObjects.Count < BulletTotal)  // Check active against allowed
        {
            GameObject newObject = Instantiate(SummonedObject, SummonLocation.position, Quaternion.identity);
            activeObjects.Add(newObject);  // Add the new bullet to the list
        }
        else
        {
            Debug.Log("Summon limit reached");
        }
        CurrentSpeed = MoveSpeed;  // Reset speed after shooting
    }

    void Melee()
    {
        GameObject newObject = Instantiate(MeleeObject, MeleeLocation.position, Quaternion.identity);
        //  Will update later to match shooting, but for now just spawns melee object   
    }

    void Movement()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        float direction = Mathf.Sign(target.position.x - transform.position.x);
        if (distance < detectionRange)
        {
            playerdetected = true;  // Set player detected to true if within detection range
        }
        else if (distance > detectionRange)
        {
            playerdetected = false;
            Patrol();  // Baseline when no player, will instead patrol
        }
        if (playerdetected == true)
            {   
                if (facingRight)
                {
                    Flip();
                }
                if (!facingRight){
                EnemyRB.linearVelocity = new Vector2(direction * ChaseSpeed, EnemyRB.linearVelocity.y);  // Move the enemy towards the target
                }
            }
    }

    void Patrol()
    {
        EnemyRB.linearVelocity = new Vector2(MoveSpeed, EnemyRB.linearVelocity.y);  // Move the enemy horizontally based on the current speed
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        HitWall = Physics2D.Raycast(transform.position, Vector2.left, 1f, groundLayer);
        HitWallR = Physics2D.Raycast(transform.position, Vector2.right, 1f, groundLayer);
            if(!isGrounded && facingRight)
            {
                Flip();
            }
            else if (!isGrounded && !facingRight)
            {
                Flip();
            }
            if (HitWall || HitWallR)           
            {
                Flip();
            }
        }
    }
    private void MoveTowards(Vector3 target1)
    {
        Vector2 direction = (target1 - transform.position).normalized;  // Calculate the direction towards the target
        transform.position += (Vector3)direction * ChaseSpeed * Time.deltaTime;
    }

    void Flip()
{   delay += Time.deltaTime;
    if (delay > 0.4f)  // Add a delay to prevent rapid flipping
    {
    delay = 0;
    facingRight = !facingRight;
    transform.Rotate(new Vector3(0, 180, 0));
    MoveSpeed = -MoveSpeed;
}   }
}

