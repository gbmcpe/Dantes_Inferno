using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Health : MonoBehaviour
{
    public int health; // hp value
    public int maxHP = 10;  // Max hp
    public bool invuln = false;  // Indicates if the player is currently invulnerable
   
    
    
    void Start()
    {
        health = maxHP;
    }
    void Update()
    {

    }


    public void TakeDamage(int Damage)
    {
        if (invuln == false)
        {
            StartCoroutine(Invulntime());
            health -= Damage;
            if (health <= 0)
            {
                
                // Info for death, decide later

            }
            
            //  Have damage apply to hp here
        }
    }
    IEnumerator Invulntime()
    {   
        invuln = true;
        yield return new WaitForSeconds(1f);
        invuln = false;
    }
}