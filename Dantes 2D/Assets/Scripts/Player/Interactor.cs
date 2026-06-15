using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{


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
            

            Collider2D iObj = Physics2D.OverlapCircle(transform.position, InteractRange, InteractableLayer);

            if (iObj != null && iObj.gameObject.TryGetComponent(out IInteractable interactobj) && interactobj.CanInteract() == true)
            {
                print("if statement fired" + interactobj.ToString());
                interactobj.Interact(this);
            }
        }
        
    }
}
