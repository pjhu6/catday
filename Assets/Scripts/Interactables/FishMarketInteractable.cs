using UnityEngine;
using System.Collections;

public class FishMarketInteractable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Fish Market Interactable Clicked");
    }

    public override void Interact()
    {
        StartCoroutine(TriggerMissionAfterDialogue());
    }

    public override bool IsInteractable()
    {
        return MissionManager.Instance.IsMissionCompleted("boxcat2") &&
               !MissionManager.Instance.IsMissionCompleted("fish_market");
    }

    private IEnumerator TriggerMissionAfterDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        MissionManager.Instance.CompleteMission("fish_market");
    }
}
