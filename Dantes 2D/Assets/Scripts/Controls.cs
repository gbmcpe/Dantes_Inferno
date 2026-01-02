using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Controls : MonoBehaviour
{

    #region Variables

    private Rigidbody2D player;

    [SerializeField] private float hSpeed;
    [SerializeField] private float vSpeed;

    private bool grounded = true;
    #endregion

    #region Awake, Start, Update
    // Awake is called at game initialization
    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        pMovement();
    }

    #endregion

    #region Voids

    //Player Movement Method
    private void pMovement()
    {
        //Strafing
        player.linearVelocity = new Vector3(Input.GetAxis("Horizontal") * hSpeed, player.linearVelocity.y, 0);


        //Jumping
        if (Input.GetKey(KeyCode.Space)
            && grounded)
        {
            player.linearVelocity = new Vector3(player.linearVelocity.x, vSpeed, 0);
            grounded = false;
        }

    }

    //Camera Movement Method
    private void cMovement()
    {

    }

    //Interact Button
    private void Interact()
    {

    }


    #endregion

    #region collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collision detected");
        if (collision.gameObject.tag == "Ground") 
        {
            grounded = true;
            print("touched grass");


        }

        #endregion
    }
}
