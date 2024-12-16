using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class FloorManager : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject coinPrefab;
    public int numberOfTiles = 5;
    public float tileLength = 20f;
    public Transform playerTransform;

    [Header("Coin Settings")]
    public int coinsPerTile = 3;
    public float coinHeight = 1f;
    public float coinSpreadWidth = 6f;

    [Header("Obstacle Settings")]
    public GameObject laneObstaclePrefab;    // Single lane obstacle
    public GameObject barrierObstaclePrefab;  // Full width obstacle
    public float barrierHeight = 1f;  // Height player can jump over
    public int obstaclesPerTile = 2;
    public float obstacleHeight = 1f;
    public float[] lanes = new float[] { -2f, 0f, 2f };

    [Header("Safe Start")]
    public int safeStartTiles = 3;
    private int currentTileIndex = 0;

    private List<GameObject> activeTiles = new List<GameObject>();
    private float spawnPosition = 0f;
    private float recyclePosition;

    public static FloorManager Instance;

    void Awake()
    {
        Instance = this;
    }


    void SpawnTile()
    {
        GameObject tile = Instantiate(floorPrefab, new Vector3(0, 0, spawnPosition), Quaternion.identity);

        // Only spawn obstacles after safe start tiles
        if (currentTileIndex >= safeStartTiles)
        {
            SpawnObstaclesOnTile(tile);
        }

        SpawnCoinsOnTile(tile);
        activeTiles.Add(tile);
        spawnPosition += tileLength;
        currentTileIndex++;
    }

    void SpawnCoinsOnTile(GameObject tile)
    {
        for (int i = 0; i < coinsPerTile; i++)
        {
            float randomX = Random.Range(-coinSpreadWidth / 2, coinSpreadWidth / 2);
            float randomZ = Random.Range(0, tileLength);
            Vector3 coinPosition = tile.transform.position +
                                 new Vector3(randomX, coinHeight, randomZ);

            // Check for overlap with obstacles
            bool overlap = false;
            foreach (Transform child in tile.transform)
            {
                if (child.CompareTag("Obstacle"))
                {
                    float minDist = 1.5f; // Minimum distance between coin and obstacle
                    if (Vector3.Distance(new Vector3(coinPosition.x, 0, coinPosition.z),
                                      new Vector3(child.position.x, 0, child.position.z)) < minDist)
                    {
                        overlap = true;
                        break;
                    }
                }
            }

            if (!overlap)
            {
                GameObject coin = Instantiate(coinPrefab, coinPosition, Quaternion.identity);
                coin.transform.SetParent(tile.transform, worldPositionStays: true);
            }
        }
    }

    public void ResetFloor()
    {
        currentTileIndex = 0;

        // Clear existing tiles
        foreach (GameObject tile in activeTiles)
        {
            Destroy(tile);
        }
        activeTiles.Clear();

        // Reset positions
        spawnPosition = 0f;
        recyclePosition = tileLength * 2;

        // Spawn initial tiles
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
        recyclePosition = tileLength * 2;
    }

    void Update()
    {
        if (playerTransform.position.z >= recyclePosition)
        {
            RecycleTile();
            recyclePosition += tileLength;
        }
    }

    void RecycleTile()
    {
        GameObject oldestTile = activeTiles[0];
        activeTiles.RemoveAt(0);

        // Destroy existing coins and obstacles
        foreach (Transform child in oldestTile.transform)
        {
            if (child.CompareTag("Coin") || child.CompareTag("Obstacle"))
            {
                Destroy(child.gameObject);
            }
        }

        oldestTile.transform.position = new Vector3(0, 0, spawnPosition);
        SpawnCoinsOnTile(oldestTile);
        SpawnObstaclesOnTile(oldestTile);
        spawnPosition += tileLength;

        activeTiles.Add(oldestTile);
    }
    void SpawnObstaclesOnTile(GameObject tile)
    {
        float floorY = tile.transform.position.y;

        for (int i = 0; i < obstaclesPerTile; i++)
        {
            // Randomly choose obstacle type
            bool isBarrier = Random.value > 0.5f;

            float randomZ = Random.Range(tileLength * 0.2f, tileLength * 0.8f);

            if (isBarrier)
            {
                // Spawn full-width barrier that must be jumped over
                Vector3 barrierPosition = new Vector3(
                    tile.transform.position.x,
                    floorY + obstacleHeight,
                    tile.transform.position.z + randomZ
                );

                GameObject barrier = Instantiate(barrierObstaclePrefab, barrierPosition, Quaternion.identity);
                barrier.transform.SetParent(tile.transform, worldPositionStays: true);
            }
            else
            {
                // Spawn lane obstacle that can be avoided by moving left/right
                float lanePosition = lanes[Random.Range(0, lanes.Length)];
                Vector3 obstaclePosition = new Vector3(
                    tile.transform.position.x + lanePosition,
                    floorY + obstacleHeight,
                    tile.transform.position.z + randomZ
                );

                GameObject obstacle = Instantiate(laneObstaclePrefab, obstaclePosition, Quaternion.identity);
                obstacle.transform.SetParent(tile.transform, worldPositionStays: true);
            }
        }
    }
}

