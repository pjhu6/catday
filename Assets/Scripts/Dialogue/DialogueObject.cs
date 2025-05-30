using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    public DialogueData dialogueData;

    public void TriggerDialogue()
    {
        // Get the singleton instance of DialogueManager
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
    }
}

