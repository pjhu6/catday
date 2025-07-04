using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;


public class IntroVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image loadingImage;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started += OnVideoStarted;
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("VideoPlayer is not assigned in IntroVideoPlayer.");
        }
    }

    private void OnVideoStarted(VideoPlayer vp)
    {
        loadingImage.enabled = false;
        videoPlayer.started -= OnVideoStarted;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Load MainScene after the video ends
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started -= OnVideoStarted;
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
