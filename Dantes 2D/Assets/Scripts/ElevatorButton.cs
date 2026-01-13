using UnityEngine;

public class ElevatorButton : MonoBehaviour
{

    [SerializeField] Vector2 eDirection;
    [SerializeField] float eSpeed;
    [SerializeField] float eDistance;

    private float endposition;

    public GameObject eFloor;
    private Rigidbody2D floorMover;

    private bool activated = false;

    private void Start()
    {
        floorMover = eFloor.GetComponent<Rigidbody2D>();

        endposition = floorMover.position.y;
        endposition += eDistance;

    }

    // Update is called once per frame
    void Update()
    {
        if (activated == true && floorMover.position.y < eDistance)
        {

            floorMover.linearVelocity = eDirection;
    
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            activated = true;
             transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
        }
    }


}
