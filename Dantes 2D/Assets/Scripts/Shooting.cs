using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletSpawn;

    private float delay;
    private GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");  // Find player, allows change to targets
    }

    // Update is called once per frame
    void Update()
    {
        delay += Time.deltaTime;
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if(distance < 20)
        {
            delay += Time.deltaTime;

             if(delay > 2)
            {
                delay = 0;
                Shoot();
            } 
        }
    }

    void Shoot()
    {
        Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
    }
}
