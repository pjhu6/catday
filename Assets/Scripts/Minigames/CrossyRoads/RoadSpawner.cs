using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnPercentage = 0.8f;

    private Camera mainCamera;
    private GridManager gridManager;

    private List<GameObject> activeRoads = new List<GameObject>();
    private HashSet<int> usedGridY = new HashSet<int>(); // Track grid Y-values we've already spawned on

    private void Start()
    {
        mainCamera = Camera.main;
        gridManager = GridManager.Instance;
    }

    private void Update()
    {
        if (!FindObjectOfType<IntroPlayer>().IsIntroFinished())
        {
            return;
        }

        Vector2 topCenter = mainCamera.ViewportToWorldPoint(new Vector2(0.5f, 1.1f));
        Vector2Int gridPos = gridManager.WorldToGrid(topCenter);

        if (!usedGridY.Contains(gridPos.y) && Random.value < spawnPercentage)
        {
            SpawnRoad(gridPos);
        }
        CleanRoads();
    }

    private void SpawnRoad(Vector2Int gridPos)
    {
        Vector2 spawnPos = gridManager.GridToWorld(gridPos);
        GameObject road = Instantiate(roadPrefab, spawnPos, Quaternion.identity, transform);
        activeRoads.Add(road);

        // Mark this grid Y as used
        usedGridY.Add(gridPos.y);
    }

    private void CleanRoads()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(Vector2.zero);

        for (int i = activeRoads.Count - 1; i >= 0; i--)
        {
            GameObject road = activeRoads[i];
            if (road == null) continue;

            if (road.transform.position.y < bottomLeft.y - 2f) // 2f buffer
            {
                int gridY = gridManager.WorldToGrid(road.transform.position).y;

                // Free up that grid line for future spawns
                usedGridY.Remove(gridY);

                Destroy(road);
                activeRoads.RemoveAt(i);
            }
        }
    }
}
