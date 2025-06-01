using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool IsDialogueActive { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private Image nextImage;
    [SerializeField] private AudioSource audioSource;

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
    private bool isInFade = false;
    private bool isInDialogueAnimation = false;

    private float dialogueEndTime;

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
        // Show next line on space or mouse click
        if (IsDialogueActive
        && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        && !isInFade
        && !isInDialogueAnimation)
        {
            ShowNextLine();
        }
    }


    public void StartDialogue(DialogueData data)
    {
        currentLineIndex = 0;
        currentLines = data.dialogueLines;
        cutsceneImages = data.cutsceneImages;
        cutsceneAudios = data.cutsceneAudios;

        setActive(true);

        // If there's a cutscene to show, do the fade-in effect
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
        if (currentLineIndex < currentLines.Length)
        {
            DialogueLine currentLine = currentLines[currentLineIndex];
            bool isLastLine = currentLineIndex == currentLines.Length - 1;
            StartCoroutine(TypeAnimation(currentLine.line, isLastLine));

            // Set speaker image and name
            speakerNameText.text = currentLine.speakerName;

            // Play audio if available
            foreach (var cutscene in cutsceneAudios)
            {
                if (cutscene.dialogueStartIndex == currentLineIndex)
                {
                    audioSource.clip = cutscene.audio;
                    audioSource.Play();
                    break;
                }
            }

            // Check if there is a cutscene to show
            foreach (var cutscene in cutsceneImages)
            {
                if (cutscene.dialogueStartIndex == currentLineIndex)
                {
                    cutscenePanel.SetActive(true);
                    cutsceneImage.sprite = cutscene.image;
                    Debug.Log("Showing cutscene image: " + currentLineIndex);
                    break;
                }
            }
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        setActive(false);
        cutscenePanel.SetActive(false);
        speakerNameText.text = "";
        dialogueText.text = "";
        nextImage.enabled = false;
        // Set the next time actions can be performed
        dialogueEndTime = Time.time + dialogueCooldown;
    }

    private void setActive(bool isActive)
    {
        dialoguePanel.SetActive(isActive);
        IsDialogueActive = isActive;
    }

    private IEnumerator FadeInFromBlack()
    {
        isInFade = true;
        blackOverlay.gameObject.SetActive(true);
        Color color = blackOverlay.color;
        color.a = 1f;
        blackOverlay.color = color;

        // Delay for a moment before starting the fade
        yield return new WaitForSeconds(fadeDelay);

        // Start next line as we're fading in
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
            // Use carat to indicate a pause in the dialogue
            if (c == '^')
            {
                yield return new WaitForSeconds(pauseDelay);
                continue; // Skip the pause character
            }
            dialogueText.text += c;
            yield return new WaitForSeconds(letterDelay); // Adjust typing speed here
        }
        // make nextImage bob up and down repeatedly until next line is shown
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
            // Bob the image up and down
            if (elapsedTime >= Mathf.PI * 2) // Reset after a full cycle
            {
                elapsedTime = 0f;
            }
            elapsedTime += Time.deltaTime * bobSpeed;
            float newY = originalPosition.y + Mathf.Sin(elapsedTime) * bobHeight;
            nextImage.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
            yield return null;
        }

        // Reset position when dialogue ends or is inactive
        nextImage.transform.localPosition = originalPosition;
    }
    
    public bool InDialogue()
    {
        return IsDialogueActive || Time.time < dialogueEndTime;
    }
}
