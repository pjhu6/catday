using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Texture2D[] frames;         // Use textures instead of sprites
    public float frameRate = 0.1f;
    public bool loop = true;
    public bool autoPlay = true;

    private int currentFrame = 0;
    private float timer = 0f;
    private bool isPlaying = false;

    private MeshRenderer meshRenderer;
    private Material materialInstance;

    private Texture2D originalTexture;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            // Get a unique material instance to avoid affecting shared material
            materialInstance = meshRenderer.material;

            // Save the original texture (assuming shader uses _BaseMap)
            originalTexture = materialInstance.GetTexture("_BaseMap") as Texture2D;
        }
        else
        {
            Debug.LogWarning("SpriteAnimator: No MeshRenderer found on GameObject.");
        }
    }

    void Start()
    {
        if (autoPlay)
            Play();
    }

    void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0 || materialInstance == null) return;

        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            currentFrame++;
            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                    SetFrame(currentFrame);
                }
                else
                {
                    Stop(); // Resets to original texture
                    return;
                }
            }
            else
            {
                SetFrame(currentFrame);
            }

            timer = 0f;
        }
    }

    public void Play()
    {
        if (frames == null || frames.Length == 0 || materialInstance == null) return;

        currentFrame = 0;
        timer = 0f;
        isPlaying = true;
        SetFrame(currentFrame);
    }

    public void Stop()
    {
        isPlaying = false;
        // Return to original texture
        if (materialInstance != null && originalTexture != null)
            materialInstance.SetTexture("_BaseMap", originalTexture);
    }

    private void SetFrame(int index)
    {
        if (materialInstance != null && frames != null && index < frames.Length)
        {
            materialInstance.SetTexture("_BaseMap", frames[index]);
        }
    }
}
