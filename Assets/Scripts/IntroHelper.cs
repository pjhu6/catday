using UnityEngine;
using System.Collections;
using System;

public class IntroHelper : MonoBehaviour
{
    public static IntroHelper Instance { get; private set; }

    public DialogueData dialogueData;
    public Boolean skipIntro = false;

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
        if (skipIntro)
        {
            MissionManager.Instance.DisplayMissionPanel();
            return;
        }

        // Disable mission initially, just for the intro
        MissionManager.Instance.HideMissionPanel();

        DialogueManager.Instance.StartDialogue(dialogueData);
        
        // Wait until the dialogue is finished before displaying the mission panel
        StartCoroutine(ShowMissionsAfterIntro());
    }

    private IEnumerator ShowMissionsAfterIntro()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        MissionManager.Instance.DisplayMissionPanel();
    }
}
