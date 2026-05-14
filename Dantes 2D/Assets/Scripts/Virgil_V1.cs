using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Virgil_V1 : MonoBehaviour
{
    public Transform target;
    public float acceleration;
    public bool isEvil;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, acceleration * Time.deltaTime);
        }
    }
    void VisualChange()
    {
        if (isEvil == true)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
    

