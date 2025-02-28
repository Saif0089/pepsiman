using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Environment Pool Settings")]
    public GameObject environmentPatchPrefab;
    public int EnvironmentPoolSize = 5;

    public List<GameObject> ObstaclePrefabs;
    public List<GameObject> CollectablePrefabs;
    public List<GameObject> EnviornmentCashTemplates;
    public GameObject BoosterBag;

    public int ObstaclePoolSize = 10;
    public int CollectablePoolSize = 10;
    public int CashTemplatePoolSize = 5;

    private Queue<GameObject> ObstacleObjectPool = new Queue<GameObject>();
    private Queue<GameObject> CollectableObjectPool = new Queue<GameObject>();
    private Queue<GameObject> environmentPool = new Queue<GameObject>();

    List<GameObject> activeEnvironmentPatches = new List<GameObject>();
    [HideInInspector] public List<GameObject> bag;

    private void Start()
    {
        // Initialize Obstacle Pool
        for (int i = 0; i < ObstaclePoolSize; i++)
        {
            int randomIndex = Random.Range(0, ObstaclePrefabs.Count);
            GameObject obj = Instantiate(ObstaclePrefabs[randomIndex]);
            obj.SetActive(false);
            ObstacleObjectPool.Enqueue(obj);
        }

        // Initialize Collectable Pool
        for (int i = 0; i < CollectablePoolSize; i++)
        {
            int randomIndex = Random.Range(0, CollectablePrefabs.Count);
            GameObject obj = Instantiate(CollectablePrefabs[randomIndex]);
            obj.SetActive(false);
            CollectableObjectPool.Enqueue(obj);
        }

        // Initialize Environment Pool
        for (int i = 0; i < EnvironmentPoolSize; i++)
        {
            GameObject obj = Instantiate(environmentPatchPrefab);
            obj.SetActive(false);
            environmentPool.Enqueue(obj);
        }
    }

    public GameObject SpawnObstacleFromPool(Vector3 position, Quaternion rotation)
    {
        if (ObstacleObjectPool.Count == 0)
        {
            Debug.LogWarning("No obstacles left in the pool.");
            return null;
        }

        GameObject objectToSpawn = ObstacleObjectPool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = new Vector3(0, position.y, position.z);
        objectToSpawn.transform.rotation = rotation;

        ObstacleObjectPool.Enqueue(objectToSpawn); // Re-add to pool
        return objectToSpawn;
    }

    public GameObject SpawnCollectableFromPool(Vector3 position, Quaternion rotation)
    {
        if (CollectableObjectPool.Count == 0)
        {
            Debug.LogWarning("No collectables left in the pool.");
            return null;
        }

        GameObject objectToSpawn = CollectableObjectPool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        CollectableObjectPool.Enqueue(objectToSpawn); // Re-add to pool
        return objectToSpawn;
    }

    public void SpawnBoosterBag()
    {
        if (activeEnvironmentPatches.Count < 3 || PlayerController.instance.BoostEnabled || bag.Count != 0) 
        {
            Debug.LogWarning("Not enough active environment patches to spawn the bag.");
            return;
        }

        // Sort the patches based on the z position in ascending order
        activeEnvironmentPatches.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

        // Choose the 2nd or 3rd patch (adjust based on preference)
        int patchIndex = Mathf.Min(2, activeEnvironmentPatches.Count - 1); // Ensure we don't go out of bounds
        GameObject selectedPatch = activeEnvironmentPatches[patchIndex];

        if (selectedPatch == null)
        {
            Debug.LogWarning("Could not determine the correct patch to spawn the bag.");
            return;
        }

        Vector3 spawnPosition = selectedPatch.transform.position + new Vector3(0, 1.1f, 0);

        GameObject spawnedBag = Instantiate(BoosterBag, spawnPosition, BoosterBag.transform.rotation);
        spawnedBag.transform.SetParent(selectedPatch.transform);
        bag.Add(spawnedBag);
    }
    
  public void SpawnCashTemplatesInEnvironment()
{
    if (activeEnvironmentPatches == null || activeEnvironmentPatches.Count == 0)
    {
        Debug.LogWarning("No active environment patches available for spawning cash templates.");
        return;
    }

    if (EnviornmentCashTemplates == null || EnviornmentCashTemplates.Count == 0)
    {
        Debug.LogWarning("No cash templates assigned in EnvironmentCashTemplates.");
        return;
    }

    foreach (GameObject patch in activeEnvironmentPatches)
    {
        if (patch == null)
        {
            Debug.LogWarning("Found a null patch in activeEnvironmentPatches. Skipping.");
            continue;
        }

        EnvironmentPatch patchComponent = patch.GetComponent<EnvironmentPatch>();
        if (patchComponent == null || patchComponent.CashTemplatesSpawnPos == null || patchComponent.CashTemplatesSpawnPos.Count == 0)
        {
            Debug.LogWarning($"Patch {patch.name} has no defined spawn positions. Skipping.");
            continue;
        }

        // Filter out null spawn points
        var validSpawnPoints = patchComponent.CashTemplatesSpawnPos.Where(pos => pos != null).ToList();
        if (validSpawnPoints.Count == 0)
        {
            Debug.LogWarning($"Patch {patch.name} has only null spawn positions. Skipping.");
            continue;
        }

        // Select a random spawn point from the valid list
        int randomPointIndex = Random.Range(0, validSpawnPoints.Count);
        Vector3 spawnPosition = validSpawnPoints[randomPointIndex].position;

        // Select a random cash template and instantiate it
        int randomIndex = Random.Range(0, EnviornmentCashTemplates.Count);
        var cashTemplate = Instantiate(EnviornmentCashTemplates[randomIndex], spawnPosition, Quaternion.identity, patch.transform);
    }
}
    public GameObject GetActiveEnvironmentPatch()
    {
        if (environmentPool.Count == 0)
        {
            Debug.LogWarning("No available environment patches in the pool.");
            return null;
        }

        GameObject patch = environmentPool.Dequeue();
        patch.SetActive(true);
        activeEnvironmentPatches.Add(patch);
        return patch;
    }

    public void ReturnActiveEnvironmentPatch(GameObject patch)
    {
        if (activeEnvironmentPatches.Contains(patch))
        {
            activeEnvironmentPatches.Remove(patch);
            environmentPool.Enqueue(patch);
            patch.SetActive(false);
            EnvironmentManager.Instance.checkSpawn();
        }
    }
}
