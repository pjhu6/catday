using UnityEngine;
using UnityEngine.Video;


public class IntroVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("VideoPlayer is not assigned in IntroVideoPlayer.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Load MainScene after the video ends
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
