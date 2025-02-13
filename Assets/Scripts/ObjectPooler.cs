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
    public int ObstaclePoolSize = 10;
    public int CollectablePoolSize = 10;

    private Queue<GameObject> ObstacleObjectPool;
    private Queue<GameObject> CollectableObjectPool;
    private Queue<GameObject> environmentPool = new Queue<GameObject>();

    private void Start()
    {
        ObstacleObjectPool = new Queue<GameObject>();
        for (int i = 0; i < ObstaclePoolSize; i++)
        {
            int randomIndex = Random.Range(0, ObstaclePrefabs.Count);
            GameObject obj = Instantiate(ObstaclePrefabs[randomIndex]);
            obj.SetActive(false);
            ObstacleObjectPool.Enqueue(obj);
        }
        
        CollectableObjectPool = new Queue<GameObject>();
        for (int i = 0; i < CollectablePoolSize; i++)
        {
            int randomIndex = Random.Range(0, CollectablePrefabs.Count);
            GameObject obj = Instantiate(CollectablePrefabs[randomIndex]);
            obj.SetActive(false);
            CollectableObjectPool.Enqueue(obj);
        }
        
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
            Debug.LogWarning("No objects left in the pool.");
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
            Debug.LogWarning("No objects left in the pool.");
            return null;
        }

        GameObject objectToSpawn = CollectableObjectPool.Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        CollectableObjectPool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    
    public GameObject GetEnvironmentPatch()
    {
        GameObject patch = environmentPool.Dequeue();
        patch.SetActive(true);
        return patch;
    }

    public void ReturnEnvironmentPatch(GameObject patch)
    {
        environmentPool.Enqueue(patch);
        patch.SetActive(false);
        EnvironmentManager.Instance.checkSpawn();
    }
}