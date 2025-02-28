using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensures only one instance exists
            return;
        }
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
    public float sphereRadius = 1.0f;
    public float maxDistance = 5.0f;
    public LayerMask layerMask; // Optional: Set a layer mask to limit the sphere cast

    public float boosterBagSpawnInterval = 5f; // Time interval for spawning booster bags
    private float boosterBagTimer;

    private float cashTemplateTimer;
    private void Start()
    {
        // Initial spawn of obstacles
        for (float z = initialSpawnZ; z > 0; z -= distanceBetweenObstacles)
        {
            SpawnObstacle(z);
        }

        // Initialize timers
        boosterBagTimer = boosterBagSpawnInterval;
    }

    private void Update()
    {
        if (!canSpawn) return;

        // Handle obstacle spawning
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObstacle(spawnZ);
            spawnTimer = spawnInterval;
        }

        // Handle booster bag spawning
        boosterBagTimer -= Time.deltaTime;
        if (boosterBagTimer <= 0f && !PlayerController.instance.BoostEnabled)
        {
            SpawnBoosterBag();
            boosterBagTimer = boosterBagSpawnInterval;
        }
    }

    private void SpawnObstacle(float zPosition)
    {
        float randomX = Random.Range(minX, maxX); // Vary X position
        Vector3 spawnPosition = new Vector3(randomX, 0, zPosition);

        if (IsPositionClear(spawnPosition) && ObjectPooler.Instance != null)
        {
            SpawnManager.Instance.MarkPositionOccupied(randomX, zPosition);
            ObjectPooler.Instance.SpawnObstacleFromPool(spawnPosition, Quaternion.identity);
        }
    }

    public void SpawnBoosterBag()
    {
        float randomX = Random.Range(minX, maxX);
        float boosterZPosition = spawnZ;
        Vector3 spawnPosition = new Vector3(randomX, 0, boosterZPosition);

        if (IsPositionClear(spawnPosition) && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.SpawnBoosterBag();
        }
    }

    public void StopSpawning(bool state)
    {
        canSpawn = state;
    }

    public bool IsPositionClear(Vector3 position)
    {
        return !Physics.CheckSphere(position, sphereRadius, layerMask);
    }
}
