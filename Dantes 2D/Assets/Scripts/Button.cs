using UnityEngine;

public class Button : MonoBehaviour
{

    interface IInteractable
    {
        public void Interact();
    }
    public bool activation;

    public GameObject output1;
    public GameObject output2;
    public GameObject output3;
    public GameObject output4;


    void Start()
    {

        ColorUpdate(activation);

    }


    public void Interact()
    {
        activation = !activation;

        if (activation == true)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);

        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);

        }

    }


    private void ColorUpdate(bool activ)
    {
        if (activ == true) { transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0); }
        else { transform.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0); }
    }


}
