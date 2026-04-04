using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Health : MonoBehaviour
{
    public int health; // hp value
    public int maxHP;  // Max hp
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
            if (health > maxHP)
            {
                health = maxHP;  // Prevents overheal
            }
        }
    }
    IEnumerator Invulntime()
    {   
        invuln = true;
        yield return new WaitForSeconds(1f);
        invuln = false;
    }
}