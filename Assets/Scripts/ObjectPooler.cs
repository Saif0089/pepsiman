using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
            obj.name = "EnvironmentPatch_" + (i + 1);
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

        activeEnvironmentPatches.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

        int patchIndex = Mathf.Min(2, activeEnvironmentPatches.Count - 1);
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
            bool isFirstPatch = activeEnvironmentPatches[0] == patch;

            activeEnvironmentPatches.Remove(patch);
            environmentPool.Enqueue(patch);
            patch.SetActive(false);

            if (isFirstPatch) 
            {
                EnvironmentPatch patchToReturn = patch.GetComponent<EnvironmentPatch>();
                foreach (GameObject cash in patchToReturn.AllCashTemplates)
                {
                    cash.SetActive(true);
                }
            }
            EnvironmentManager.Instance.checkSpawn();
        }
    }

    [ContextMenu("ResetGame")]
    
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
