using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    public bool Activation;
    private BoxCollider2D DoorCol;
    private SpriteRenderer DoorRender;

    void Start()
    {
        DoorCol = gameObject.GetComponent<BoxCollider2D>();
        DoorRender = gameObject.GetComponent<SpriteRenderer>();

        DoorState();

    }

    public bool CanInteract()
    {
        throw new System.NotImplementedException();
    }


    public void Interact(Interactor interactor)
    {
        throw new System.NotImplementedException();
    }


    private void DoorState()
    { 
        
        if (Activation == true)
        {
            DoorCol.enabled = false;
            DoorRender.enabled = false;

        }
        else 
           {
            DoorCol.enabled = true;
            DoorRender.enabled = true;

        }
    }

    void IInteractable.Activate()
    {
        Activation = !Activation;
        DoorState();
    
    }
}
