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
        IsPanning = false;
        Vector3 TargetPosition = target.position + offset;
        TargetPosition.z = transform.position.z;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            TargetPosition.y += PanDistance;
            IsPanning = true;
            PanCam(TargetPosition);

        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            TargetPosition.y -= PanDistance;
            IsPanning = true;
            PanCam(TargetPosition);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TargetPosition.x -= PanDistance;
            IsPanning = true;
            PanCam(TargetPosition);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            TargetPosition.x += PanDistance;
            IsPanning = true;
            PanCam(TargetPosition);
        }

        if (IsPanning == false && transform.position == TargetPosition)
        {
            transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, Damping);
        }
        else if (IsPanning == false && transform.position != TargetPosition)
        { 
            PanCam(TargetPosition);
        }
    }

    private void PanCam(Vector3 TargetPosition)
    { 
    transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, PanDamping);
    }
    }

    