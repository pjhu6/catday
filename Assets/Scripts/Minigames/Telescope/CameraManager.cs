using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Vector3 dragOrigin;
    public float dragSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            // Dynamically fetch BackgroundImage's borders
            RectTransform backgroundRect = GameObject.Find("BackgroundImage").GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            backgroundRect.GetWorldCorners(corners);
            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];

            Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float cameraRight = transform.position.x + Camera.main.orthographicSize * Screen.width / Screen.height;
            float cameraLeft = transform.position.x - Camera.main.orthographicSize * Screen.width / Screen.height;
            float cameraTop = transform.position.y + Camera.main.orthographicSize;
            float cameraBottom = transform.position.y - Camera.main.orthographicSize;
            // Check if the camera's new position would be within the bounds of the background image
            if (cameraLeft + difference.x < bottomLeft.x || cameraRight + difference.x > topRight.x)
            {
                difference.x = 0; // Prevent horizontal movement outside bounds
            }
            if (cameraBottom + difference.y < bottomLeft.y || cameraTop + difference.y > topRight.y)
            {
                difference.y = 0; // Prevent vertical movement outside bounds
            }
            transform.position += difference * dragSpeed;
        }
    }
}
