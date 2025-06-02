using UnityEngine;

public class FishMarketInteractable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Fish Market Interactable Clicked");
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }

    public override bool IsInteractable()
    {
        return MissionManager.Instance.IsMissionCompleted("boxcat") &&
               !MissionManager.Instance.IsMissionCompleted("fish_market");
    }
}
