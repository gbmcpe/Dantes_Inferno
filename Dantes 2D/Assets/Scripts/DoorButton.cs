using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public GameObject Door;
    private BoxCollider2D DoorCol;
    private SpriteRenderer DoorRender;

    public bool Activated;

    private void Start()
    {
        DoorCol = Door.GetComponent<BoxCollider2D>();
        DoorRender = Door.GetComponent<SpriteRenderer>();

    }

    private void Update()
    {

        if (Activated)
        {
            DoorCol.enabled = false;
            DoorRender.enabled = false;

        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            DoorCol.enabled = false;
            DoorRender.enabled = false;

            transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
        }

    }
}

