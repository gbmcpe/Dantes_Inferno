using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DamageOHeal : MonoBehaviour
{
    public int Damage = 1;
    public Health playerHealth;
    void Start()
    {

    }
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerHealth = collision.gameObject.GetComponent<Health>();
            playerHealth.TakeDamage(Damage); 

        }
        
    }
    
}