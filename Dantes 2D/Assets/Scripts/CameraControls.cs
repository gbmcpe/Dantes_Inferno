using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    [SerializeField] private Vector3 offset;
    [SerializeField] private float Damping;

    [SerializeField] private float PanDamping;

    [SerializeField] private float PanDistance;

    private bool IsPanning = false;

    public Transform target;

    private Vector3 Velocity = Vector3.zero;

    private void FixedUpdate()
    {
        
        Vector3 TargetPosition = target.position + offset;
        TargetPosition.z = transform.position.z;


        //Look Up
        if (Input.GetKey(KeyCode.UpArrow))
        {
            TargetPosition.y += PanDistance;
            IsPanning = true;
            SmoothCam(TargetPosition, true);
        }

        //Look Down
        if (Input.GetKey(KeyCode.DownArrow))
        {
            TargetPosition.y -= PanDistance;
            IsPanning = true;
            SmoothCam(TargetPosition, true);
        }

        //Look Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TargetPosition.x -= PanDistance;
            IsPanning = true;
            SmoothCam(TargetPosition, true);
        }

        //Look Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            TargetPosition.x += PanDistance;
            IsPanning = true;
            SmoothCam(TargetPosition, true);
        }

        //case where the camera has been panned and needs to be returned to player
        if (IsPanning == true && transform.position != TargetPosition)
        {
        SmoothCam(TargetPosition, true); 
        
        }

         //Resets Panning State when it finishes returning to the player
        if (IsPanning == true && transform.position == TargetPosition)
        {
            IsPanning = false;
        
        }


        if (IsPanning == false) { SmoothCam(TargetPosition, false); }

        // if (IsPanning == false && transform.position == TargetPosition)
        // {
        //    PanCam(TargetPosition, false);
        //}
        // else if (IsPanning == false && transform.position != TargetPosition)
        // { 
        //     PanCam(TargetPosition, true);
        // }
    }

    private void SmoothCam(Vector3 TargetPosition, bool Pan)
    {
    
        if (Pan == true)
        {
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, PanDamping);
        }
        else
        {
         transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, Damping);
        }

    }
    }

    