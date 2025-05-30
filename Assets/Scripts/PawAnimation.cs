using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PawAnimation : MonoBehaviour
{
    public Image pawImage;
    public Sprite pawFrame1;
    public Sprite pawFrame2;

    public float frameDuration = 0.1f;    // Time between frame 1 and 2
    public float totalDuration = 0.3f;    // Total time before hiding

    private bool isAnimating = false;


    void Update()
    {
        // Check if the dialogue is not active and the mouse is clicked, and not already animating
        if (!DialogueManager.Instance.InDialogue() && Input.GetMouseButtonDown(0) && !isAnimating)
        {
            StartCoroutine(PlayPawAnimation());
        }
    }

    IEnumerator PlayPawAnimation()
    {
        isAnimating = true;

        // Frame 1
        pawImage.sprite = pawFrame1;
        pawImage.enabled = true;

        yield return new WaitForSeconds(frameDuration);

        // Frame 2
        pawImage.sprite = pawFrame2;

        yield return new WaitForSeconds(totalDuration - frameDuration);

        // Hide
        pawImage.enabled = false;
        isAnimating = false;
    }
}
