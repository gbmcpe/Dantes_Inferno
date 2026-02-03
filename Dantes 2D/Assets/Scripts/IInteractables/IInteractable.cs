
public interface IInteractable
{

    //Activate is used when another object activates an object. 
    //It can optionally pass an int as data to be interpreted by the recipient object.
    //ex: For the Elevator Call Buttons, 1 designates an elevator as the START button, and 2 as the END button.
    public void Activate(int ActivData = 0);

    //Whether an object is currently interactable or not.
    public bool CanInteract();

    //The method used when the Player initiates an interaction.
    public void Interact(Interactor interactor);
    
}
