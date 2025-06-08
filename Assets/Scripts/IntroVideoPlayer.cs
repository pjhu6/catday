using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class IntroVideoPlayer : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void PlayOverlayVideo(string url);
#endif

    [SerializeField] private string videoUrl = "myclip.mp4";
    [SerializeField] private float videoDuration = 10f;

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayOverlayVideo(videoUrl);
        Invoke(nameof(LoadMainScene), videoDuration);
#else
        Debug.LogWarning("IntroVideoPlayer is intended for WebGL builds only.");
        LoadMainScene();
#endif
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
