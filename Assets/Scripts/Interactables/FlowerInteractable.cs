using UnityEngine;

public class FlowerInteractable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Flower Clicked");
    }

    public override void Interact()
    {
        Debug.Log("Flower Interactable Interacted");
        StartCoroutine(TriggerDialogueAndCompleteMission());
    }

    private System.Collections.IEnumerator TriggerDialogueAndCompleteMission()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        MissionManager.Instance.CompleteMission("explore_park");
    }
}
