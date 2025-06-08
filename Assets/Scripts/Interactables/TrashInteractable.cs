using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrashInteractable : AbstractInteractable
{
    public float tipAngle = 90f; // degrees
    public float tipDuration = 1f; // seconds
    public float objectHeight = 1f; // height of the trash can

    public DialogueData dialogueData;

    public override void Click()
    {
        Debug.Log("Trash Interactable Clicked");
        StartCoroutine(TipOverThenDialogue());
    }

    public override void Interact()
    {
        Debug.Log("Trash Interactable Interacted");
    }

    public override bool IsInteractable()
    {
        return MissionManager.Instance.IsMissionCompleted("boxcat1") &&
               !MissionManager.Instance.IsMissionCompleted("boxcat2");
    }

    private IEnumerator TipOverThenDialogue()
    {
        yield return StartCoroutine(TipFromBottom());
        yield return new WaitForSeconds(0.5f);
        DialogueManager.Instance.StartDialogue(dialogueData);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        MissionManager.Instance.CompleteMission("boxcat2");
    }

    private IEnumerator TipFromBottom()
    {
        Quaternion initialRot = transform.rotation;
        Quaternion targetRot = initialRot * Quaternion.Euler(tipAngle, 0, 0);

        Vector3 pivotOffset = new Vector3(0f, -objectHeight / 2f, 0f); // adjust to pivot at bottom
        Vector3 worldPivot = transform.position + transform.TransformVector(pivotOffset);

        this.isEnabled = false; // Disable interaction during tipping
        float elapsed = 0f;
        while (elapsed < tipDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / tipDuration);

            Quaternion currentRot = Quaternion.Slerp(initialRot, targetRot, t);

            // Rotate around the bottom pivot point
            transform.rotation = currentRot;
            transform.position = worldPivot - transform.TransformVector(pivotOffset);
            yield return null;
        }

        transform.rotation = targetRot;
        transform.position = worldPivot - transform.TransformVector(pivotOffset);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
}
