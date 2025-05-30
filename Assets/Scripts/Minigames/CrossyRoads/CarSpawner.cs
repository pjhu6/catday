using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] carPrefabs;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float minSpawnInterval = 2f;
    [SerializeField] private float maxSpawnInterval = 5f;
    [SerializeField] private float carSpacing = 1.5f;
    [SerializeField] private float carYSpacing = 0.5f;

    [Header("Initial Cars")]
    [SerializeField] private int minInitialCarCount = 1;
    [SerializeField] private int maxInitialCarCount = 5;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        SpawnInitialCars();

        StartCoroutine(SpawnCarLoop());
    }

    private void SpawnInitialCars()
    {
        if (carPrefabs.Length == 0 || minInitialCarCount <= 0) return;

        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
        float y = transform.position.y;

        int initialCarCount = Random.Range(minInitialCarCount, maxInitialCarCount + 1);

        // List to keep track of used X positions
        List<float> occupiedPositions = new List<float>();

        for (int i = 0; i < initialCarCount; i++)
        {
            GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

            // Try to find a non-overlapping random position
            float randomX;
            int attempts = 0;
            const int maxAttempts = 20;
            bool positionFound = false;

            do
            {
                randomX = Random.Range(leftEdge.x, rightEdge.x);
                positionFound = true;
                foreach (float usedX in occupiedPositions)
                {
                    if (Mathf.Abs(usedX - randomX) < carSpacing)
                    {
                        positionFound = false;
                        break;
                    }
                }
                attempts++;
            } while (!positionFound && attempts < maxAttempts);

            // If no good position found after max attempts, just accept last one
            occupiedPositions.Add(randomX);

            bool spawnFromLeft = (randomX < (leftEdge.x + rightEdge.x) / 2);

            Vector3 spawnPos = new Vector3(randomX, spawnFromLeft ? y : y + carYSpacing, -0.1f);
            GameObject car = Instantiate(carPrefab, spawnPos, Quaternion.identity, transform);

            if (!spawnFromLeft)
            {
                Vector3 scale = car.transform.localScale;
                scale.x *= -1;
                car.transform.localScale = scale;
            }

            car.AddComponent<CarManager>().Initialize(spawnFromLeft ? Vector2.right : Vector2.left, moveSpeed);
        }
    }


    private IEnumerator SpawnCarLoop()
    {
        while (true)
        {
            SpawnCar();
            float delay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnCar()
    {
        if (carPrefabs.Length == 0) return;

        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

        bool spawnFromLeft = Random.value > 0.5f;

        float y = transform.position.y;

        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));

        Vector3 spawnPos = new Vector3(
            spawnFromLeft ? leftEdge.x - carSpacing : rightEdge.x + carSpacing,
            spawnFromLeft ? y : y + carYSpacing,
            -0.1f
        );

        GameObject car = Instantiate(carPrefab, spawnPos, Quaternion.identity, transform);

        if (!spawnFromLeft)
        {
            Vector3 scale = car.transform.localScale;
            scale.x *= -1;
            car.transform.localScale = scale;
        }

        car.AddComponent<CarManager>().Initialize(spawnFromLeft ? Vector2.right : Vector2.left, moveSpeed);
    }
}
