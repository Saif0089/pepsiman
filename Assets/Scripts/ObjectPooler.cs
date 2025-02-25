using UnityEngine;
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
    public GameObject BoosterBag;
    
    public int ObstaclePoolSize = 10;
    public int CollectablePoolSize = 10;

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

        ObstacleObjectPool.Enqueue(objectToSpawn);
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

        CollectableObjectPool.Enqueue(objectToSpawn);
        return objectToSpawn;
    }
    public void SpawnBoosterBag()
    {
        if (activeEnvironmentPatches.Count == 0 || PlayerController.instance.BoostEnabled || bag.Count !=0) 
        {
            Debug.LogWarning("No active environment patches to spawn the bag.");
            return;
        }

        GameObject forwardPatch = null;
        float maxZ = float.MinValue;

        foreach (GameObject patch in activeEnvironmentPatches)
        {
            if (patch.transform.position.z > maxZ)
            {
                maxZ = patch.transform.position.z;
                forwardPatch = patch;
            }
        }

        if (forwardPatch == null)
        {
            Debug.LogWarning("Could not determine the forward patch.");
            return;
        }

        Vector3 spawnPosition = forwardPatch.transform.position + new Vector3(0, 1.07f, 0);

        GameObject spawnedBag = Instantiate(BoosterBag, spawnPosition, BoosterBag.transform.rotation);
        spawnedBag.transform.SetParent(forwardPatch.transform);
        bag.Add(spawnedBag);
        Debug.Log("Bag Spawned on forward patch: " + forwardPatch.name);
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
