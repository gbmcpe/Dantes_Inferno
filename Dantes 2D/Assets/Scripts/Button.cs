using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    public bool CanInteract;

    public bool Activated;


    public GameObject output1;



    void Start()
    {
        ColorUpdate(Activated);
    }


    private void ColorUpdate(bool activ)
    {
        if (activ == true) { transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0); }
        else { transform.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0); }
    }

    bool IInteractable.CanInteract()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.Activate()
    {
       
throw new System.NotImplementedException();
     
    }

    void IInteractable.Interact(Interactor interactor)
    {
        print("Interaction Activated");

        Activated = !Activated;

        if (Activated == true)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);


        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);


        }

        IInteractable outputinter = output1.GetComponent<IInteractable>();

        outputinter.Activate();

    }
}