using UnityEngine;

public class CarMover : MonoBehaviour
{
    public float moveDistance = 100f;
    public float speed = 20f;
    public bool moveRight = true;
    public bool loop = true;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _moving = true;

    private void Start()
    {
        _startPosition = transform.position;

        // Determine the target position based on direction
        Vector3 direction = moveRight ? Vector3.right : Vector3.left;
        _targetPosition = _startPosition + direction * moveDistance;
    }

    private void Update()
    {
        if (_moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);

            // Check if destination reached
            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                // Instantly teleport back
                transform.position = _startPosition;

                if (!loop)
                {
                    enabled = false; // Stop updating if not looping
                }
            }
        }
    }
}
