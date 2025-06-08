using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public RectTransform backgroundImage;
    private Vector3 dragOrigin;
    public float dragSpeed = 1f;

    void Start()
    {
        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = GetMouseWorldPosition();
        }

        if (Input.GetMouseButton(0))
        {
            Vector3[] corners = new Vector3[4];
            backgroundImage.GetWorldCorners(corners);
            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];

            Vector3 currentMousePos = GetMouseWorldPosition();
            Vector3 difference = dragOrigin - currentMousePos;

            Camera mainCamera = Camera.main;
            float camHeight = mainCamera.orthographicSize;
            float camWidth = camHeight * mainCamera.aspect;

            float cameraRight = transform.position.x + camWidth;
            float cameraLeft = transform.position.x - camWidth;
            float cameraTop = transform.position.y + camHeight;
            float cameraBottom = transform.position.y - camHeight;

            if (cameraLeft + difference.x < bottomLeft.x || cameraRight + difference.x > topRight.x)
                difference.x = 0;
            if (cameraBottom + difference.y < bottomLeft.y || cameraTop + difference.y > topRight.y)
                difference.y = 0;

            transform.position += difference * dragSpeed;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Mathf.Abs(Camera.main.transform.position.z); // z-depth from camera
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
