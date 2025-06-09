using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossyRoadsPlayer : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public Vector2 startingPosition = new Vector2(0, -4);

    private Vector2 targetLocalPosition;
    private bool isMoving = false;
    private Vector2Int currentGridPosition;
    private int maxGridHeight;

    private void Start()
    {
        // Snap to grid and set initial positions
        Vector2 snappedWorldPos = GridManager.Instance.SnapToGrid(startingPosition);
        Vector2 localSnappedPos = snappedWorldPos - (Vector2)GridManager.Instance.transform.position;

        transform.localPosition = localSnappedPos;
        currentGridPosition = GridManager.Instance.WorldToGrid(snappedWorldPos);
        targetLocalPosition = transform.localPosition;
        maxGridHeight = currentGridPosition.y;
    }

    private void Update()
    {
        if (!FindObjectOfType<IntroPlayer>().IsIntroFinished())
        {
            return;
        }

        if (GridManager.Instance.IsBelowScreen(currentGridPosition))
            {
                // If the player is off the screen, reset the game
                RestartGame();
                return;
            }
        
        // Update score if player hasn't been on this grid height before
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Vector2Int newGridPos = GridManager.Instance.GetLeft(currentGridPosition);
                MoveTo(newGridPos);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Vector2Int newGridPos = GridManager.Instance.GetRight(currentGridPosition);
                MoveTo(newGridPos);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Vector2Int newGridPos = GridManager.Instance.GetUpper(currentGridPosition);
                MoveTo(newGridPos);
                StartCoroutine(GridManager.Instance.ChangeSpeed(1.5f, 0.3f));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Vector2Int newGridPos = GridManager.Instance.GetLower(currentGridPosition);
                MoveTo(newGridPos);
            }
        }

        if (isMoving)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.localPosition, targetLocalPosition) < 0.01f)
            {
                transform.localPosition = targetLocalPosition;
                isMoving = false;
            }
        }

        // Check if the player has moved to a new grid height
        if (currentGridPosition.y > maxGridHeight)
        {
            maxGridHeight = currentGridPosition.y;
            GridManager.Instance.SetScore(GridManager.Instance.GetScore() + 1);
            Debug.Log("New Score: " + GridManager.Instance.GetScore());
            Debug.Log("Max Grid Height: " + maxGridHeight);
        }
    }

    private void MoveTo(Vector2Int newGridPos)
    {
        if (GridManager.Instance.IsGridPositionValid(newGridPos))
        {
            currentGridPosition = newGridPos;

            // Convert target world position to local space
            Vector2 worldTarget = GridManager.Instance.GridToWorld(newGridPos);
            targetLocalPosition = worldTarget - (Vector2)GridManager.Instance.transform.position;

            isMoving = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Car"))
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }   

    public Vector2Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }
}
