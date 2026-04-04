using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //  Movement Section
    public bool CanMove;  // Check to determine if can move
    public float MoveSpeed;  // Max speed of movement
    private float CurrentSpeed;  //  Current speed
    public bool AimToShoot;  // Decides if enemy will stop to shoot
    
    //  Sprite Section

    //  Attack Section
    public GameObject SummonedObject;  //  Bullet prefab to be instantiated when shooting
    public GameObject MeleeObject;  // Melee prefab to be instantiated when melee
    public Transform SummonLocation;  //  Location where the object will be spawned
    public Transform MeleeLocation;  // Location where the melee will be spawned
    public int MaxHp;  // Maximum health
    public int currentHp; // Current health
    public bool CanShoot;  // Check to determine if can melee
    public int FireDelay;  // Time between shots in seconds
    public int GunDistance; // Maximum distance starting to shoot
    public int BulletTotal; // Total number of bullets allowed at once
    private List<GameObject> activeObjects = new List<GameObject>();  // List to keep track of summoned objects
    public bool CanMelee;  // Check to determine if can melee
    public int MeleeDelay;  // Time between melee attacks in seconds
    public int MeleeDistance; // Maximum distance starting to melee
    private float delay;
    private GameObject target;  // Reference to the player or target

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");  // Find player, allows change to targets
        currentHp = MaxHp;  // Set current health to maximum health at the start
        CurrentSpeed = MoveSpeed;  // Set current speed to max speed at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }

        delay += Time.deltaTime;
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if(distance < GunDistance && distance > MeleeDistance && CanShoot == true)
        {
            delay += Time.deltaTime;

             if(delay > FireDelay)
            {
                delay = 0;
                Shoot();
            } 
        }
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


    void Shoot()
    {
        if (AimToShoot == true)
                {
                    CurrentSpeed = 0;
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
        GameObject newObject = Instantiate(MeleeObject, MeleeLocation.position, Quaternion.identity);    }
}
