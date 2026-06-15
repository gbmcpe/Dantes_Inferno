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

    private float PanSpeedCounter;

    private bool IsPanning = false;

    private bool PanReturn = false;

    public Transform target;

    private Vector3 Velocity = Vector3.zero;

    private void FixedUpdate()
    {

        Vector3 TargetPosition = target.position + offset;
        TargetPosition.z = transform.position.z;
        
        if (Input.GetKey(KeyCode.LeftArrow)
         || Input.GetKey(KeyCode.UpArrow)
         || Input.GetKey(KeyCode.DownArrow)
         || Input.GetKey(KeyCode.RightArrow))
        {
            PanSpeedCounter = 0;
            IsPanning = true;
            PanReturn = false;
        }

        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            TargetPosition.y += PanDistance * 0.5f;
            TargetPosition.x += PanDistance;
            SmoothCam(TargetPosition, true);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            TargetPosition.y -= PanDistance * 0.5f;
            TargetPosition.x += PanDistance;
            SmoothCam(TargetPosition, true);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            TargetPosition.y -= PanDistance * 0.5f;
            TargetPosition.x -= PanDistance;
            SmoothCam(TargetPosition, true);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            TargetPosition.y += PanDistance * 0.5f;
            TargetPosition.x -= PanDistance;
            SmoothCam(TargetPosition, true);
        }
        else if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            TargetPosition.y += PanDistance * 0.5f;
            SmoothCam(TargetPosition, true);

        }
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            TargetPosition.x += PanDistance;
            SmoothCam(TargetPosition, true);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            TargetPosition.y -= PanDistance * 0.5f;
            SmoothCam(TargetPosition, true);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            TargetPosition.x -= PanDistance;
            SmoothCam(TargetPosition, true);
        }
        else if (!Input.GetKey(KeyCode.LeftArrow)
         && !Input.GetKey(KeyCode.UpArrow)
         && !Input.GetKey(KeyCode.DownArrow)
         && !Input.GetKey(KeyCode.RightArrow) && IsPanning == true && transform.position != TargetPosition)
        {
            IsPanning = false;
            PanSpeedCounter += 1;
            PanReturn = true;

            SmoothCam(TargetPosition, true);

            print("Camera Returning!");
        }
        else if (PanReturn == true && Vector3.Distance(transform.position, TargetPosition) < 0.1)
        {
            IsPanning = false;
            PanReturn = false;
            print("Camera State Reset!");
            SmoothCam(TargetPosition, false); 
        }

        //case for normal camera state; no panning
        else if (IsPanning == false) { SmoothCam(TargetPosition, false); }
    }



    private void SmoothCam(Vector3 TargetPosition, bool Pan)
    {

        if (Pan == true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, PanDamping-(PanSpeedCounter*0.01f));
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, Damping);
        }

    }
    }

    