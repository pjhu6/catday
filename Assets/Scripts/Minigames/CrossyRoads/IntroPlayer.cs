using UnityEngine;
using TMPro;
using System.Collections;

public class IntroPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private float introDuration = 5f;

    private float timer = 0f;
    private bool introFinished = false;

    void Start()
    {
        introText.gameObject.SetActive(true);
        StartCoroutine(StartIntro());
    }

    private IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(introDuration);
        for (int i = 3; i >= 1; i--)
        {
            introText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        introText.text = "Go!";
        yield return new WaitForSeconds(1);
        introFinished = true;

        if (introText != null)
        {
            introText.gameObject.SetActive(false);
        }
    }

    public bool IsIntroFinished()
    {
        return introFinished;
    }
}
