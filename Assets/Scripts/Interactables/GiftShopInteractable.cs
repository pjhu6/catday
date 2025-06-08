using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GiftShopInteractable : AbstractInteractable
{
    public DialogueData dialogueData;
    public Image blackScreenImage;

    public override void Click()
    {
        Debug.Log("Gift Shop clicked");
    }

    public override void Interact()
    {
        StartCoroutine(StartGameAfterDialogue());
    }

    private System.Collections.IEnumerator StartGameAfterDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        // Show a black screen to avoid showing world while loading.
        blackScreenImage.gameObject.SetActive(true);
        MissionManager.Instance.CompleteMission("art_store");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("TelescopeGame");
    }

    public override bool IsInteractable()
    {
        // Only if fish market mission is completed
        return MissionManager.Instance.IsMissionCompleted("fish_market");
    }
}
