using UnityEngine;
using System.Collections;

public class IntroHelper : MonoBehaviour
{
    public static IntroHelper Instance { get; private set; }

    public DialogueData dialogueData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Disable mission initially, just for the intro
        MissionManager.Instance.HideMissionPanel();

        DialogueManager.Instance.StartDialogue(dialogueData);
        
        // Wait until the dialogue is finished before displaying the mission panel
        StartCoroutine(WaitForDialogueEnd());
    }

    private IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        MissionManager.Instance.DisplayMissionPanel();
    }
}
