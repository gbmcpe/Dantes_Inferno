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

    private bool PanReturn = false;

    public Transform target;

    private Vector3 Velocity = Vector3.zero;

    private void FixedUpdate()
    {
        
        Vector3 TargetPosition = target.position + offset;
        TargetPosition.z = transform.position.z;

        

        //Look Up
        if (Input.GetKey(KeyCode.UpArrow))
        {
            TargetPosition.y += PanDistance * 0.5f;
            IsPanning = true;
            PanReturn = false;
            SmoothCam(TargetPosition, true);
        }

        //Look Down
        if (Input.GetKey(KeyCode.DownArrow))
        {
            TargetPosition.y -= PanDistance*0.5f;
            IsPanning = true;
            PanReturn = false;
            SmoothCam(TargetPosition, true);
        }

        //Look Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TargetPosition.x -= PanDistance;
            IsPanning = true;
            PanReturn = false;
            SmoothCam(TargetPosition, true);
        }

        //Look Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            TargetPosition.x += PanDistance;
            IsPanning = true;
            PanReturn = false;
            SmoothCam(TargetPosition, true);
        }

        if (Input.anyKeyDown == false && IsPanning == true)
        {
            PanReturn = true;
        }
        

        //case where the camera has been panned and needs to be returned to player
            if (PanReturn == true && transform.position != TargetPosition)
            {
                SmoothCam(TargetPosition, true);

                print("Camera Returning!");
            }

        //case for Panning State when it finishes returning to the player
        if (PanReturn == true && Vector3.Distance(transform.position, TargetPosition) < 0.1)
        {
            IsPanning = false;
            PanReturn = false;
            print("Camera State Reset!");
            SmoothCam(TargetPosition, false); 
        }

        //case for normal camera state; no panning
        if (IsPanning == false) { SmoothCam(TargetPosition, false); }
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

    