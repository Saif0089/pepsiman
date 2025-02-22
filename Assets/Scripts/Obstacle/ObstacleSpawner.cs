using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance;

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
    public float sphereRadius = 1.0f;
    public float maxDistance = 5.0f;
    public LayerMask layerMask; // Optional: Set a layer mask to limit the sphere cast

    private void Start()
    {
        // Initial spawn of obstacles
        for (float z = initialSpawnZ; z > 0; z -= distanceBetweenObstacles)
        {
            SpawnObstacle(z);
        }
    }
    public bool IsPositionClear(Vector3 position)
    {
        // Perform a sphere cast at the given position
        RaycastHit hit;
        var cols= Physics.OverlapSphere(position, sphereRadius,layerMask ,QueryTriggerInteraction.UseGlobal);

        pos = position;
        // Check if the hit object has the tag "Obstacle"
        if (cols.Length>0 )
        {
            return false; // Obstacle found
        }

        return true; // No obstacle found
    }
   
    Vector3 pos;
    private void OnDrawGizmos()
    {
      
        // Draw the sphere.
     //   Gizmos.DrawSphere(pos,sphereRadius);

      
    }
    private void Update()
    {
        if (!canSpawn) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObstacle(spawnZ);
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnObstacle(float zPosition)
    {
        float randomX;
        int maxAttempts = 5;
        int attempts = 0;

        //do
        //{
        //    // randomX = Random.Range(minX, maxX);
        //    randomX = 0;
        //    attempts++;
        //}
        //while (SpawnManager.Instance.IsPositionOccupied(randomX, zPosition) && attempts < maxAttempts);

        Vector3 spawnPosition = new Vector3(0, 0, zPosition);
        if (IsPositionClear(spawnPosition))
        {
            SpawnManager.Instance.MarkPositionOccupied(0, zPosition);

            ObjectPooler.Instance.SpawnObstacleFromPool(spawnPosition, Quaternion.identity);
        }
    }


    public void StopSpawning(bool state)
    {
        canSpawn = state;
    }
}