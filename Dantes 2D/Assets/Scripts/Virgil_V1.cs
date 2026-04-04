using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Virgil_V1 : MonoBehaviour
{
    public Transform target;
    public float acceleration;

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
}
    

