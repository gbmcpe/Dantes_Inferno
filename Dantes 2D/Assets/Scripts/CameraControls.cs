using UnityEngine;

public class CameraControls : MonoBehaviour
{

    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

   public Transform target;

   private Vector3 Velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 TargetPosition = target.position+offset;
        TargetPosition.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref Velocity, damping);

    }
}
    