using UnityEngine;

public class BigTreeInteractable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Big Tree Clicked");
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
