using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool IsDialogueActive { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private Image nextImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource nextAudioSource;
    [SerializeField] private AudioSource backgroundAudioSource;

    [SerializeField] private GameObject cutscenePanel;
    [SerializeField] private Image cutsceneImage;

    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeDelay = 0.5f;
    [SerializeField] private float letterDelay = 0.05f;
    [SerializeField] private float pauseDelay = 0.1f;

    public float dialogueCooldown = 0.2f;

    private DialogueLine[] currentLines;
    private int currentLineIndex;
    private CutsceneImages[] cutsceneImages;
    private CutsceneAudio[] cutsceneAudios;
    private AudioClip backgroundMusic;
    private bool isInFade = false;
    private bool isInDialogueAnimation = false;
    private Coroutine typeAnimationCoroutine; // Added to hold reference to the type animation

    private float dialogueEndTime;

    private Coroutine cutsceneImageSwapCoroutine;
    private Coroutine autoplayCoroutine;
    private Sprite currentCutsceneImage;
    private Sprite currentSecondaryImage;
    private float currentAlternateDuration = 1f;
    private bool isAlternatingCutsceneImage = false;
    private bool isInCutscene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        setActive(false);
        dialogueEndTime = Time.time;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (IsDialogueActive && autoplayCoroutine == null && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isInFade)
        {
            // If currently animating text, complete it immediately
            if (isInDialogueAnimation)
            {
                if (typeAnimationCoroutine != null)
                {
                    StopCoroutine(typeAnimationCoroutine);
                    typeAnimationCoroutine = null;
                }
                CompleteTypeAnimation();
            }
            // Otherwise, move to the next line
            else
            {
                nextAudioSource.Play();
                ShowNextLine();
            }
        }
    }

    public void StartDialogue(DialogueData data)
    {
        currentLineIndex = 0;
        currentLines = data.dialogueLines;
        cutsceneImages = data.cutsceneImages;
        cutsceneAudios = data.cutsceneAudios;
        backgroundMusic = data.backgroundMusic;

        setActive(true);

        if (cutsceneImages.Length > 0)
        {
            StartCoroutine(FadeInFromBlack());
        }
        else
        {
            ShowNextLine();
        }
    }

    public void ShowNextLine()
    {
        // If first line, start background music
        if (currentLineIndex == 0 && backgroundMusic != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.clip = backgroundMusic;
            backgroundAudioSource.loop = true;
            backgroundAudioSource.Play();
        }

        if (autoplayCoroutine != null)
        {
            StopCoroutine(autoplayCoroutine);
            autoplayCoroutine = null;
        }

        // Stop any existing type animation coroutine
        if (typeAnimationCoroutine != null)
        {
            StopCoroutine(typeAnimationCoroutine);
            typeAnimationCoroutine = null;
        }

        if (currentLineIndex < currentLines.Length)
        {
            // Stop any existing audio
            audioSource.Stop();

            DialogueLine currentLine = currentLines[currentLineIndex];
            bool isLastLine = currentLineIndex == currentLines.Length - 1;

            dialoguePanel.SetActive(!string.IsNullOrEmpty(currentLine.line));
            speakerNameText.text = currentLine.speakerName;

            if (!string.IsNullOrEmpty(currentLine.line))
            {
                typeAnimationCoroutine = StartCoroutine(TypeAnimation(currentLine.line, isLastLine));
            }

            AudioClip nextClip = null;
            float audioDelay = 0f;
            foreach (var cutscene in cutsceneAudios)
            {
                if (cutscene.dialogueStartIndex == currentLineIndex)
                {
                    nextClip = cutscene.audio;
                    audioDelay = cutscene.delay;
                    break;
                }
            }

            if (nextClip != null)
            {
                StartCoroutine(PlayAudio(nextClip, audioDelay));
            }

            CutsceneImages matchedCutscene = null;
            foreach (var cutscene in cutsceneImages)
            {
                if (cutscene.dialogueStartIndex == currentLineIndex)
                {
                    matchedCutscene = cutscene;
                    cutscenePanel.SetActive(true);

                    if (cutscene.secondaryImage != null)
                    {
                        currentCutsceneImage = cutscene.image;
                        currentSecondaryImage = cutscene.secondaryImage;
                        currentAlternateDuration = Mathf.Max(0.1f, cutscene.alternateDuration);
                        isAlternatingCutsceneImage = true;

                        if (cutsceneImageSwapCoroutine != null)
                            StopCoroutine(cutsceneImageSwapCoroutine);

                        cutsceneImageSwapCoroutine = StartCoroutine(AlternateCutsceneImage());
                    }
                    else
                    {
                        isAlternatingCutsceneImage = false;
                        if (cutsceneImageSwapCoroutine != null)
                            StopCoroutine(cutsceneImageSwapCoroutine);
                        cutsceneImage.sprite = cutscene.image;
                    }
                    break;
                }
            }

            if (matchedCutscene == null)
            {
                isAlternatingCutsceneImage = false;
                if (cutsceneImageSwapCoroutine != null)
                {
                    StopCoroutine(cutsceneImageSwapCoroutine);
                    cutsceneImageSwapCoroutine = null;
                }
            }

            if (matchedCutscene != null && matchedCutscene.isAutoplay)
            {
                float waitTime = Mathf.Max(0.1f, matchedCutscene.autoplayDuration);
                autoplayCoroutine = StartCoroutine(AutoplayNextLineAfterDelay(waitTime, isLastLine));
            }

            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator PlayAudio(AudioClip audioClip, float delay)
    {
        Debug.Log("Playing audio for line: " + currentLineIndex);
        yield return new WaitForSeconds(delay);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private IEnumerator AutoplayNextLineAfterDelay(float delay, bool isLastLine)
    {
        yield return new WaitForSeconds(delay);
        autoplayCoroutine = null;

        if (isLastLine)
        {
            EndDialogue();
        }
        else
        {
            ShowNextLine();
        }
    }

    private void EndDialogue()
    {
        if (cutscenePanel.activeSelf)
        {
            cutscenePanel.SetActive(false);
            MissionManager.Instance.DisplayMissionPanel();
        }

        setActive(false);
        speakerNameText.text = "";
        dialogueText.text = "";
        nextImage.enabled = false;
        audioSource.Stop();
        audioSource.clip = null;

        if (cutsceneImageSwapCoroutine != null)
        {
            StopCoroutine(cutsceneImageSwapCoroutine);
            cutsceneImageSwapCoroutine = null;
        }

        if (autoplayCoroutine != null)
        {
            StopCoroutine(autoplayCoroutine);
            autoplayCoroutine = null;
        }

        // Stop background music
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
            backgroundAudioSource.clip = null;
        }

        isInCutscene = false;
        isAlternatingCutsceneImage = false;
        dialogueEndTime = Time.time + dialogueCooldown;
    }

    private void setActive(bool isActive)
    {
        dialoguePanel.SetActive(isActive);
        IsDialogueActive = isActive;
    }

    private IEnumerator FadeInFromBlack()
    {
        isInCutscene = true;
        MissionManager.Instance.HideMissionPanel();

        isInFade = true;
        blackOverlay.gameObject.SetActive(true);
        Color color = blackOverlay.color;
        color.a = 1f;
        blackOverlay.color = color;

        yield return new WaitForSeconds(fadeDelay);

        ShowNextLine();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackOverlay.color = color;
            yield return null;
        }

        color.a = 0f;
        blackOverlay.color = color;
        blackOverlay.gameObject.SetActive(false);
        isInFade = false;
    }

    private IEnumerator TypeAnimation(string line, bool isLastLine)
    {
        nextImage.enabled = false;
        isInDialogueAnimation = true;

        // Prepare step list: pairs of (char, shouldPauseBefore)
        List<(char c, bool pauseBefore)> steps = new();
        bool pauseNext = false;
        foreach (char c in line)
        {
            if (c == '^')
            {
                pauseNext = true;
            }
            else
            {
                steps.Add((c, pauseNext));
                pauseNext = false;
            }
        }

        // Construct display string (only the actual characters)
        string displayLine = new string(steps.Select(pair => pair.c).ToArray());
        dialogueText.text = displayLine;
        dialogueText.ForceMeshUpdate();

        TMP_TextInfo textInfo = dialogueText.textInfo;
        int totalVisibleCharacters = textInfo.characterCount;
        Color32[] newVertexColors;

        // Make all characters transparent
        for (int i = 0; i < totalVisibleCharacters; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            newVertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                newVertexColors[vertexIndex + j].a = 0;
            }
        }

        dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Reveal one character at a time
        for (int i = 0; i < steps.Count; i++)
        {
            if (i >= totalVisibleCharacters)
                break;

            if (steps[i].pauseBefore)
            {
                yield return new WaitForSeconds(pauseDelay);
            }

            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            newVertexColors = textInfo.meshInfo[meshIndex].colors32;

            float elapsed = 0f;
            float fadeTime = letterDelay;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                byte alpha = (byte)Mathf.Lerp(0, 255, elapsed / fadeTime);
                for (int j = 0; j < 4; j++)
                {
                    newVertexColors[vertexIndex + j].a = alpha;
                }
                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }

            // Ensure fully visible
            for (int j = 0; j < 4; j++)
            {
                newVertexColors[vertexIndex + j].a = 255;
            }
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        isInDialogueAnimation = false; // Mark animation as complete
        typeAnimationCoroutine = null; // Clear coroutine reference
        if (!isLastLine)
        {
            StartCoroutine(DoNextImageAnimation());
        }
    }

    private void CompleteTypeAnimation()
    {
        // This method will be called when the user clicks during typing.
        // It should immediately make all text visible and play the audio.
        if (currentLineIndex > 0 && currentLineIndex <= currentLines.Length)
        {
            DialogueLine currentLine = currentLines[currentLineIndex - 1]; // Get the current line that was being typed

            // Make all text visible
            dialogueText.text = new string(currentLine.line.Where(c => c != '^').ToArray());
            dialogueText.ForceMeshUpdate();
            TMP_TextInfo textInfo = dialogueText.textInfo;
            int totalVisibleCharacters = textInfo.characterCount;

            for (int i = 0; i < totalVisibleCharacters; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                Color32[] newVertexColors = textInfo.meshInfo[meshIndex].colors32;

                for (int j = 0; j < 4; j++)
                {
                    newVertexColors[vertexIndex + j].a = 255; // Make fully opaque
                }
            }
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // Play the current audio immediately if it's not already playing
            AudioClip currentClip = null;
            foreach (var cutscene in cutsceneAudios)
            {
                if (cutscene.dialogueStartIndex == currentLineIndex - 1) // Match the current line index
                {
                    currentClip = cutscene.audio;
                    break;
                }
            }

            if (currentClip != null && audioSource.clip != currentClip)
            {
                audioSource.Stop();
                audioSource.clip = currentClip;
                audioSource.Play();
            } else if (currentClip != null && !audioSource.isPlaying) {
                // If it's the right clip but not playing, play it
                audioSource.Play();
            }

            isInDialogueAnimation = false; // Animation is now complete
            if (currentLineIndex -1 < currentLines.Length - 1) // If it's not the very last line
            {
                StartCoroutine(DoNextImageAnimation());
            }
        }
    }


    private IEnumerator DoNextImageAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        nextImage.enabled = true;
        Vector3 originalPosition = nextImage.transform.localPosition;
        float bobHeight = 3f;
        float bobSpeed = 5f;
        float elapsedTime = 0f;

        while (IsDialogueActive && !isInFade && nextImage.enabled)
        {
            if (elapsedTime >= Mathf.PI * 2)
                elapsedTime = 0f;

            elapsedTime += Time.deltaTime * bobSpeed;
            float newY = originalPosition.y + Mathf.Sin(elapsedTime) * bobHeight;
            nextImage.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
            yield return null;
        }

        nextImage.transform.localPosition = originalPosition;
    }

    private IEnumerator AlternateCutsceneImage()
    {
        bool toggle = false;

        while (IsDialogueActive && isAlternatingCutsceneImage)
        {
            cutsceneImage.sprite = toggle ? currentCutsceneImage : currentSecondaryImage;
            toggle = !toggle;
            yield return new WaitForSeconds(currentAlternateDuration);
        }
    }

    public bool InDialogue()
    {
        return IsDialogueActive || Time.time < dialogueEndTime;
    }

    public bool IsInCutscene()
    {
        return isInCutscene;
    }
}