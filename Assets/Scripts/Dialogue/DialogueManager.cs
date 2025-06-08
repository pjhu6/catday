using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
        if (IsDialogueActive
            && autoplayCoroutine == null
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            && !isInFade
            && !isInDialogueAnimation)
        {
            nextAudioSource.Play();
            ShowNextLine();
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
                StartCoroutine(TypeAnimation(currentLine.line, isLastLine));
            }

            AudioClip nextClip = null;
            foreach (var cutscene in cutsceneAudios)
            {
                if (cutscene.dialogueStartIndex == currentLineIndex)
                {
                    nextClip = cutscene.audio;
                    break;
                }
            }

            if (nextClip != null)
            {
                Debug.Log("Playing audio for line: " + currentLineIndex);
                audioSource.clip = nextClip;
                audioSource.Play();
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
        dialogueText.text = "";
        foreach (char c in line)
        {
            if (c == '^')
            {
                yield return new WaitForSeconds(pauseDelay);
                continue;
            }
            dialogueText.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        if (!isLastLine)
        {
            StartCoroutine(DoNextImageAnimation());
        }

        isInDialogueAnimation = false;
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
