using UnityEngine;

public class InteractableDebugger : MonoBehaviour
{
    [SerializeField] AbstractInteractable interactable;
    [SerializeField] bool isEnabled = false;

    void Start()
    {
        // interact with the interactable if it is enabled
        if (isEnabled)
        {
            interactable.Interact();
        }
    }
}
