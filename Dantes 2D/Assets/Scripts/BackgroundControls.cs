using UnityEngine;

public class BackgroundControls : MonoBehaviour
{
    private float startPos;
    public GameObject camera;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }
    
    void FixedUpdate()
    {
        float distance = camera.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
