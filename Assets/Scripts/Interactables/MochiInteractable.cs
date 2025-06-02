using UnityEngine;

public class MochiInteractable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Mochi clicked");
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
