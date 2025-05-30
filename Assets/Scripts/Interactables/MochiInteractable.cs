using UnityEngine;

public class MochiInteractable : AbstractInteractable
{
    public override void Click()
    {
        Debug.Log("Mochi clicked");
    }

    public override void Interact()
    {
        Debug.Log("Interacting with Mochi");
    }
}
