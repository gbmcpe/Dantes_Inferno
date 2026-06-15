using UnityEngine;



public class LedgeDetector : MonoBehaviour
{

    [SerializeField] private float ledgeCheckRadius;
    public bool canDetect;
    public MovementControls player;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    

        if (canDetect)
            {
            player.isLedgeDetected = Physics2D.OverlapCircle(transform.position, ledgeCheckRadius, player.groundLayer);    
            }
        
    }

    private void OnDrawGizmos()
    {
    Gizmos.DrawWireSphere(transform.position, ledgeCheckRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
        canDetect = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
        canDetect = true;
        }    
    }
} 

