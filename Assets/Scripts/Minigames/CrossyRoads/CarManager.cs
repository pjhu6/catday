using UnityEngine;

public class CarManager : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private Camera mainCamera;
    private float offscreenBuffer = 0.5f;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }


    public void Initialize(Vector2 moveDirection, float moveSpeed)
    {
        direction = moveDirection;
        speed = moveSpeed;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        // Destroy car if it's far off-screen
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -offscreenBuffer || viewportPos.x > 1 + offscreenBuffer)
        {
            Destroy(gameObject);
        }
    }
}
