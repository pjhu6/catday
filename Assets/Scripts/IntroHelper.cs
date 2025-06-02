using UnityEngine;
using System.Collections;
using System;

public class IntroHelper : MonoBehaviour
{
    public static IntroHelper Instance { get; private set; }

    public DialogueData dialogueData;
    public CanvasGroup tutorialCanvasGroup;
    public float tutorialDuration = 8f;
    public bool skipIntro = false;
    public bool playMusicDuringIntro = false;

    private bool isIntroCompleted = false;

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
            isIntroCompleted = true;
            MissionManager.Instance.DisplayMissionPanel();
            return;
        }
        // Disable/enable bgm in intro
        MusicManager.Instance.ToggleMusic(playMusicDuringIntro);
        

        // Disable mission initially, just for the intro
        MissionManager.Instance.HideMissionPanel();

        // Disable the intro canvas group
        tutorialCanvasGroup.alpha = 0f;
        tutorialCanvasGroup.interactable = false;
        tutorialCanvasGroup.blocksRaycasts = false;

        DialogueManager.Instance.StartDialogue(dialogueData);

        // Wait until the dialogue is finished before displaying the mission panel
        StartCoroutine(ShowMissionsAfterIntro());
    }

    private IEnumerator ShowMissionsAfterIntro()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);

        // Enable music
        MusicManager.Instance.ToggleMusic(true);

        // Enable the tutorial canvas group
        tutorialCanvasGroup.interactable = true;
        tutorialCanvasGroup.blocksRaycasts = true;
        tutorialCanvasGroup.gameObject.SetActive(true);

        // Fade in the intro canvas group
        float elapsedTime = 0f;
        float fadeDuration = 1f; // Duration for the fade-in effect
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tutorialCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        tutorialCanvasGroup.alpha = 1f;
        // Wait for the intro duration
        yield return new WaitForSeconds(tutorialDuration - fadeDuration);

        // Fade out the intro canvas group
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tutorialCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }
        tutorialCanvasGroup.alpha = 0f;
        tutorialCanvasGroup.interactable = false;
        tutorialCanvasGroup.blocksRaycasts = false;
        tutorialCanvasGroup.gameObject.SetActive(false);

        // Now display the mission panel after dialogue and tutorial
        MissionManager.Instance.DisplayMissionPanel();
        isIntroCompleted = true;
    }

    public bool IsIntroCompleted()
    {
        return isIntroCompleted;
    }
}
