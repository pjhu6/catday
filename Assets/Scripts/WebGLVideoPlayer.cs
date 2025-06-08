using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLVideoPlayer : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void PlayOverlayVideo(string url);
#endif

    public void PlayVideo(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayOverlayVideo(url);
#else
        Debug.Log("Playing video: " + url);
        // You can use UnityEngine.VideoPlayer here for non-WebGL testing
#endif
    }
}
