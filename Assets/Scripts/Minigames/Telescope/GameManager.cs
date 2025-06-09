using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DialogueData outroDialogue;
    public RectTransform[] targetImages;
    public Image crosshairImage;
    public Sprite centeredSprite;
    public TextMeshProUGUI scoreText;

    public float requiredTime = 2f;
    public float xThreshold = 50f;  // New X threshold
    public float yThreshold = 50f;  // New Y threshold

    private float timer = 0f;
    private float flashTimer = 0f;
    private bool[] targetFound;
    private bool hasWon = false;
    private Sprite originalSprite;
    private int currentTargetIndex = -1;

    void Start()
    {
        GameObject paws = GameObject.Find("Paws");
        if (paws != null)
        {
            paws.SetActive(false);
        }
        GameObject meowAudio = GameObject.Find("MeowAudio");
        if (meowAudio != null)
        {
            meowAudio.SetActive(false);
        }

        originalSprite = crosshairImage.sprite;
        targetFound = new bool[targetImages.Length];
    }

    void Update()
    {
        if (hasWon) return;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        int centeredTargetIndex = -1;

        // Check which target (if any) is currently centered
        for (int i = 0; i < targetImages.Length; i++)
        {
            if (targetFound[i]) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetImages[i].position);

            // Check x and y independently against thresholds
            float xDiff = Mathf.Abs(screenPos.x - screenCenter.x);
            float yDiff = Mathf.Abs(screenPos.y - screenCenter.y);

            if (xDiff < xThreshold && yDiff < yThreshold)
            {
                centeredTargetIndex = i;
                break;
            }
        }

        if (centeredTargetIndex != -1)
        {
            // Target is centered
            if (currentTargetIndex != centeredTargetIndex)
            {
                // Started looking at a new target
                currentTargetIndex = centeredTargetIndex;
                timer = 0f;
            }

            flashTimer += Time.deltaTime;
            if (flashTimer >= 0.2f)
            {
                crosshairImage.sprite = (crosshairImage.sprite == centeredSprite) ? originalSprite : centeredSprite;
                flashTimer = 0f;
            }

            timer += Time.deltaTime;

            if (timer >= requiredTime)
            {
                targetFound[centeredTargetIndex] = true;
                currentTargetIndex = -1;
                timer = 0f;
                crosshairImage.sprite = originalSprite;
                UpdateFoundCountText();

                if (AllTargetsFound())
                {
                    hasWon = true;
                    crosshairImage.sprite = centeredSprite;
                    StartCoroutine(FinishGame());
                }
            }
        }
        else
        {
            currentTargetIndex = -1;
            timer = 0f;
            crosshairImage.sprite = originalSprite;
        }
    }

    bool AllTargetsFound()
    {
        foreach (bool found in targetFound)
        {
            if (!found) return false;
        }
        return true;
    }

    void UpdateFoundCountText()
    {
        int count = 0;
        foreach (bool found in targetFound)
            if (found) count++;

        if (scoreText != null)
            scoreText.text = $"{count}/{targetFound.Length}";
    }

    IEnumerator FinishGame()
    {
        crosshairImage.gameObject.SetActive(false);
        MissionManager.Instance.CompleteMission("final_mission");
        yield return new WaitForSeconds(6f);
        DialogueManager.Instance.StartDialogue(outroDialogue);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        SceneManager.LoadScene("CreditsScene");
    }
}
