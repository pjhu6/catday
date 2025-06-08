using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public DialogueData outroDialogue;
    public RectTransform targetImage;
    public Image crosshairImage;
    public Sprite centeredSprite;
    public float requiredTime = 5f;
    public float centerThreshold = 50f;
    private float timer = 0f;
    private float flashTimer = 0f;
    private bool hasWon = false;
    private Sprite originalSprite;

    void Start()
    {
        // disable Paws animation game object
        MissionManager.Instance.CompleteMission("art_store");
        GameObject paws = GameObject.Find("Paws");
        if (paws != null)
        {
            paws.SetActive(false);
        }
        originalSprite = crosshairImage.sprite;
    }

    void Update()
    {
        if (hasWon) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetImage.position);
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        float distance = Vector2.Distance(screenPos, screenCenter);

        if (distance < centerThreshold)
        {
            // Flash between centeredSprite and originalSprite, with a delay
            flashTimer += Time.deltaTime;
            if (flashTimer >= 0.2f)
            {
                crosshairImage.sprite = (crosshairImage.sprite == centeredSprite) ? originalSprite : centeredSprite;
                flashTimer = 0f;
            }


            timer += Time.deltaTime;

            if (timer >= requiredTime)
            {
                hasWon = true;
                crosshairImage.sprite = centeredSprite; // Ensure the final sprite is the centered one
                StartCoroutine(FinishGame());
            }
        }
        else
        {
            crosshairImage.sprite = originalSprite;
            timer = 0f;
        }
    }

    IEnumerator FinishGame()
    {
        // Hide crosshair image
        crosshairImage.gameObject.SetActive(false);
        MissionManager.Instance.CompleteMission("final_mission");
        yield return new WaitForSeconds(6f);
        DialogueManager.Instance.StartDialogue(outroDialogue);
    }
}
