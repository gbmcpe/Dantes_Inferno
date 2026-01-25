using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{

    public bool canMove;
    [SerializeField] private float speed;
    [SerializeField] private Transform Start;
    [SerializeField] private Transform End;

    private Transform target;

    private bool startpoint = true;

    private int i;

    private bool Activated;

    private bool reverse;




    public void Activate(int activData = 0)
    {
        //if the button identifies itself as the START call button and the elevator is not at start, this fires
        if (activData == 1 && startpoint == false)
        {
            Activated = true;
        }
        //if the button identifies itself as the END call button and the elevator is not at the end, this fires
        else if (activData == 2 && startpoint == true)
        { Activated = true; }
        else if (activData != 1 && activData != 2) { print("elevator button not assigned position or invalid position"); }
        else { print("invalid elevator call; elevator already here"); }
    }
        
        

    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
        Activated = true;
    }



    void Update()
    {



        if (Activated)
        {



            if (Activated && startpoint == true) { target = End; }
            else { target = Start; }

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) < 0.01f) { Activated = false; startpoint = !startpoint; }

        }


        
   }




}
