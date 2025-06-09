using UnityEngine;

public class Box1Interactable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Click box 1");
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }

    public override bool IsInteractable()
    {
        return MissionManager.Instance.IsMissionCompleted("explore_park") && 
               !MissionManager.Instance.IsMissionCompleted("boxcat1");
    }
}
