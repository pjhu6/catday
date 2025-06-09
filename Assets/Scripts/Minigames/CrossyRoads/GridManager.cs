using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] public float gridSize = 1f;
    [SerializeField] public float moveSpeed = 0.5f;
    [SerializeField] public Boolean showGrid = false;
    [SerializeField] private Vector2 referencePoint = Vector2.zero; // Bottom-left starting point
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int targetScore = 5;

    private int score = 0;

    private void Update()
    {
        if (!FindObjectOfType<IntroPlayer>().IsIntroFinished())
        {
            return;
        }
        // Move the entire GridManager (and its children) downward in world space
        float moveAmount = moveSpeed * Time.deltaTime;
        scoreText.text = score.ToString();
        MoveGrid(moveAmount);

        if (score >= targetScore)
        {
            StartCoroutine(WinGame());
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void MoveGrid(float moveAmount)
    {
        transform.position += new Vector3(0, -moveAmount, 0);
        referencePoint += new Vector2(0, -moveAmount);
    }

    public bool IsBelowScreen(Vector2Int gridPosition)
    {
        Vector2 worldPos = GridToWorld(gridPosition);
        float bottomScreenY = Camera.main.ScreenToWorldPoint(Vector2.zero).y;

        return worldPos.y < bottomScreenY - gridSize * 0.5f;
    }

    public Vector2 SnapToGrid(Vector2 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);
        return GridToWorld(gridPos);
    }

    // Convert world position to grid coordinates
    public Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        // Calculate the grid position based on the reference point
        Vector2 adjustedPosition = worldPosition - referencePoint;
        int x = Mathf.FloorToInt(adjustedPosition.x / gridSize);
        int y = Mathf.FloorToInt(adjustedPosition.y / gridSize);
        return new Vector2Int(x, y);
    }

    // Convert grid coordinates to world position
    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        // Calculate the world position based on the reference point
        float x = gridPosition.x * gridSize + referencePoint.x;
        float y = gridPosition.y * gridSize + referencePoint.y;
        return new Vector2(x, y);
    }


    public bool IsGridPositionValid(Vector2Int gridPosition)
    {
        Vector2 worldPos = GridToWorld(gridPosition);

        return worldPos.x >= Camera.main.ScreenToWorldPoint(Vector2.zero).x &&
            worldPos.x <= Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
    }

    public Vector2Int GetUpper(Vector2Int currentGridPos)
    {
        return currentGridPos + new Vector2Int(0, 1);
    }
    public Vector2Int GetLower(Vector2Int currentGridPos)
    {
        return currentGridPos - new Vector2Int(0, 1);
    }
    public Vector2Int GetRight(Vector2Int currentGridPos)
    {
        return currentGridPos + new Vector2Int(1, 0);
    }
    public Vector2Int GetLeft(Vector2Int currentGridPos)
    {
        return currentGridPos - new Vector2Int(1, 0);
    }

    public float GetGridSize()
    {
        return gridSize;
    }
    public Vector2 GetReferencePoint()
    {
        return referencePoint;
    }
    public int GetScore()
    {
        return score;
    }
    public void SetScore(int newScore)
    {
        score = newScore;
    }

    // Coroutine to speed up for a duration
    public IEnumerator ChangeSpeed(float multiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;
    }

    private IEnumerator WinGame()
    {
        yield return new WaitForSeconds(0.5f);
        MissionManager.Instance.CompleteMission("crossy_roads");
        SceneManager.LoadScene("MainScene");
    }

    private void OnDrawGizmos()
    {
        if (!showGrid)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        int gridLines = 20;

        for (int i = -gridLines; i <= gridLines; i++)
        {
            // Horizontal lines
            Vector2 startH = referencePoint + new Vector2(-gridLines * gridSize, i * gridSize);
            Vector2 endH = referencePoint + new Vector2(gridLines * gridSize, i * gridSize);
            Gizmos.DrawLine(startH, endH);

            // Vertical lines
            Vector2 startV = referencePoint + new Vector2(i * gridSize, -gridLines * gridSize);
            Vector2 endV = referencePoint + new Vector2(i * gridSize, gridLines * gridSize);
            Gizmos.DrawLine(startV, endV);
        }
    }
}