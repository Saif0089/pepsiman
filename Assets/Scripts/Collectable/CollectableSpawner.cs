using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    public static CollectableSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public float spawnInterval = 2f; // Time interval between spawns
    private float spawnTimer;

    public float minX = -5f;
    public float maxX = 5f;
    public float initialSpawnZ = 20f; // Initial Z position where obstacles are spawned
    public float spawnZ = 10f; // Z position for regular spawning
    public float distanceBetweenObstacles = 5f; // Distance between obstacles
    private bool canSpawn = true; // Flag to control spawning

    private void Start()
    {
        // Initial spawn of obstacles
        for (float z = initialSpawnZ; z > 0; z -= distanceBetweenObstacles)
        {
            SpawnCollectable(z);
        }
    }

    private void FixedUpdate()
    {
        if (!canSpawn) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnCollectable(spawnZ);
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnCollectable(float zPosition)
    {
        float randomX;
        int maxAttempts = 5; // Prevent infinite loops
        int attempts = 0;

        do
        {
            randomX = Random.Range(minX, maxX);
            attempts++;
        } 
        while (SpawnManager.Instance.IsPositionOccupied(randomX, zPosition) && attempts < maxAttempts);

        Vector3 spawnPosition = new Vector3(randomX, 1, zPosition);
        SpawnManager.Instance.MarkPositionOccupied(randomX, zPosition);

        ObjectPooler.Instance.SpawnCollectableFromPool(spawnPosition, Quaternion.identity);
    }


    public void StopSpawning(bool state)
    {
        canSpawn = state;
    }
}
