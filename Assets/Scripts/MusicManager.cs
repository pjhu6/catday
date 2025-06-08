using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip walkingMusic;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private bool isEnabled = true;
    private static readonly string mainSceneName = "MainScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != mainSceneName)
        {
            return;
        }

        // Stop bgm if not enabled
        if (!isEnabled)
        {
            FadeMusicOut();
            return;
        }

        // Stop bgm if in a cutscene
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsInCutscene())
        {
            FadeMusicOut();
            return;
        }

        // If dialogue is not active, play the bgm
        if (!audioSource.isPlaying)
        {
            audioSource.clip = walkingMusic;
            audioSource.loop = false;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically stop music if not in MainScene
        if (scene.name != mainSceneName && audioSource.isPlaying)
        {
            FadeMusicOut();
        }
    }

    private void FadeMusicOut()
    {
        if (audioSource.isPlaying)
        {
            //implement a fade-out effect here
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < 1; t += Time.deltaTime / fadeOutDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume after stopping
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = volume;
    }

    public void ToggleMusic(bool enable)
    {
        isEnabled = enable;
    }
}
