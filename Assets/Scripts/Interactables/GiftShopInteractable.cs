using UnityEngine;
using UnityEngine.SceneManagement;

public class GiftShopInteractable : AbstractInteractable
{
    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Gift Shop clicked");
    }

    public override void Interact()
    {
        StartCoroutine(StartGameAfterDialogue());
        Debug.Log("Switching to Telescope Game scene");
    }

    private System.Collections.IEnumerator StartGameAfterDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        SceneManager.LoadScene("TelescopeGame");
    }

    public override bool IsInteractable()
    {
        // Only if fish market mission is completed
        return MissionManager.Instance.IsMissionCompleted("fish_market");
    }
}
