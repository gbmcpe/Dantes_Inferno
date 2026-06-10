using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Section")] 
    [SerializeField] private bool CanMove;  // Check to determine if can move
    [SerializeField] private float MoveSpeed;  // Max speed of movement
    [SerializeField] private float ChaseSpeed;  // Speed when chasing the target
    [SerializeField]private float CurrentSpeed;  //  Current speed
    private Rigidbody2D EnemyRB;  // Rigidbody2D component attached to the enemy
    private bool facingRight;  // Check to determine if facing right
    private bool isGrounded;  // Check to determine if grounded
    private bool HitWall;  // Check to determine if hitting a wall
    private bool HitWallR;  // Check to determine if hitting a wall
    private bool groundBelow;  // Check to determine if there is ground below
    
    [Header("Sprite Section")] 

    [Header("Attack Section")] 


    [Header("   Shooting Sub-Section")] 
    [SerializeField] private GameObject SummonedObject;  //  Bullet prefab to be instantiated when shooting
    [SerializeField] private Transform SummonLocation;  //  Location where the object will be spawned
    [SerializeField] private bool CanShoot;  // Check to determine if can melee
    [SerializeField] private int FireDelay;  // Time between shots in seconds
    [SerializeField] private int GunDistance; // Maximum distance starting to shoot
    [SerializeField] private int BulletTotal; // Total number of bullets allowed at once
    private List<GameObject> activeObjects = new List<GameObject>();  // List to keep track of summoned objects
    [SerializeField] private bool AimToShoot;  // Decides if enemy will stop to shoot
    [SerializeField] private float StandTime;  // Time to stand still when shooting, only if AimToShoot is true
    private bool IsShooting;  // Check to determine if currently shooting, only used if AimToShoot is true    
    [Header("   Shooting Sub-Section")] 
    [SerializeField] private GameObject MeleeObject;  // Melee prefab to be instantiated when melee
    [SerializeField] private Transform MeleeLocation;  // Location where the melee will be spawned
    [SerializeField] private bool CanMelee;  // Check to determine if can melee
    [SerializeField] private int MeleeDelay;  // Time between melee attacks in seconds
    [SerializeField] private int MeleeDistance; // Maximum distance starting to melee
    [Header("HP Section")] 
    [SerializeField] private int MaxHp;  // Maximum health
    private int currentHp; // Current health
    [Header("Other Section")] 
    private float delay;  //  Delay timer for various features
    private Transform target;  // Reference to the player or target
    private bool playerdetected;  // Check to determine if player is detected
    [SerializeField] private LayerMask groundLayer; // Put ground layer here
    [SerializeField] private int detectionRange;  // Check to determine if target is in sight,
    private float CurrentXScale;  //  Current X scale of the enemy
    private float CurrentYScale;  //  Current Y scale of the enemy

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;  // Find player, allows change to targets
        currentHp = MaxHp;  // Set current health to maximum health at the start
        EnemyRB = GetComponent<Rigidbody2D>();  // Gets the Rigidbody2D component attached to the enemy
        CurrentXScale = transform.localScale.x;  //  Current X scale of the enemy
        CurrentYScale = transform.localScale.y;  //  Current Y scale of the enemy
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
        if(distance < detectionRange && distance > MeleeDistance && CanShoot == true)
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
        activeObjects.RemoveAll(item => item == null);  // Remove null entries from the list
        if(activeObjects.Count < BulletTotal)  // Check active against allowed
        {   
            if (AimToShoot == true)
            {
                delay += Time.deltaTime;
                if(delay > StandTime)  
                {   
                    delay = 0;
                    ToggleShooting();
                }
            }
            GameObject newObject = Instantiate(SummonedObject, SummonLocation.position, Quaternion.identity);
            activeObjects.Add(newObject);  // Add the new bullet to the list
        }
        else
        {
            Debug.Log("Summon limit reached");
        }
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
        {   if (IsShooting == false)
            {
                CurrentSpeed = ChaseSpeed;
            }
            if (IsShooting == true)
            {   
                CurrentSpeed = 0;
            }
            else if (facingRight)
            {
                Flip();
            }
            else if (!facingRight)
            {
                EnemyRB.linearVelocity = new Vector2(direction * CurrentSpeed, EnemyRB.linearVelocity.y);  // Move the enemy towards the target
            }
        }
    }

    void Patrol()
    {   CurrentSpeed = MoveSpeed;
        EnemyRB.linearVelocity = new Vector2(MoveSpeed, EnemyRB.linearVelocity.y);  // Move the enemy horizontally based on the current speed
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, (CurrentYScale / 2)+0.1f, groundLayer);
        HitWall = Physics2D.Raycast(transform.position, Vector2.left, (CurrentXScale / 2), groundLayer);
        HitWallR = Physics2D.Raycast(transform.position, Vector2.right, (CurrentXScale / 2), groundLayer);
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
    void ToggleShooting()
    {
        IsShooting = !IsShooting;  // Toggle the shooting state
    }
    void Flip()
    {   
    delay += Time.deltaTime;
    if (delay > 0.5f)  // Add a delay to prevent rapid flipping
    {
    delay = 0;
    facingRight = !facingRight;
    transform.Rotate(new Vector3(0, 180, 0));
    CurrentSpeed = -CurrentSpeed;
    delay += Time.deltaTime;
    if (delay > 1f && !isGrounded)  // Add a delay to prevent rapid flipping
    {
        delay = 2;
    }   
    }
}
}

