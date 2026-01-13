using UnityEngine;

public class Interactor : MonoBehaviour
{

    interface IInteractable
    {
        public void Interact();
    }

    public float InteractRange;
    public LayerMask InteractableLayer;


    void Start()
    {
        
    }


    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("e");
            // Ray r = new Ray(InteractorSource);

            Collider2D iObj = Physics2D.OverlapCircle(transform.position, InteractRange, InteractableLayer);

            if (iObj.gameObject.TryGetComponent(out IInteractable interactobj))
            {
                interactobj.Interact();
            }
        }
        
    }
}
