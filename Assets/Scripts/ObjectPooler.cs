using UnityEngine;
using DG.Tweening;
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
    [Space]

    public GameObject EnvironmentHolder;
    [Space]

    public GameObject environmentPatchPrefab;
    [Space]

    public GameObject TurnedEnvironmentPatchPrefab;
    [Space]

    public int EnvironmentPoolSize = 5;
    public GameObject BoosterBag;

    public int CollectablePoolSize = 10;

    private Queue<GameObject> CollectableObjectPool = new Queue<GameObject>();
    private Queue<GameObject> environmentPool = new Queue<GameObject>();

    public List<GameObject> activeEnvironmentPatches = new List<GameObject>();
    [HideInInspector] public List<GameObject> bag;

    private bool turnedPatchSpawned = false; // Ensure Turned Patch spawns only once
    public Vector3 nextSpawnPoint; // Add this at the top of ObjectPooler

    private void Start()
    {
        // Initialize Environment Pool
        for (int i = 0; i < EnvironmentPoolSize; i++)
        {
            GameObject obj = Instantiate(environmentPatchPrefab);
            obj.name = "EnvironmentPatch_" + (i + 1);
            obj.SetActive(false);
            obj.transform.SetParent(EnvironmentHolder.transform);
            environmentPool.Enqueue(obj);
        }
    }

    public GameObject SpawnCollectableFromPool(Vector3 position, Quaternion rotation)
    {
        if (CollectableObjectPool.Count == 0)
        {
            Debug.Log("No collectables left in the pool.");
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
            Debug.Log("Not enough active environment patches to spawn the bag.");
            return;
        }

        activeEnvironmentPatches.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

        int patchIndex = Mathf.Min(2, activeEnvironmentPatches.Count - 1);
        GameObject selectedPatch = activeEnvironmentPatches[patchIndex];

        if (selectedPatch == null)
        {
            Debug.Log("Could not determine the correct patch to spawn the bag.");
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
            Debug.Log("No available environment patches in the pool.");
            return null;
        }

        GameObject patch = environmentPool.Dequeue();
        patch.SetActive(true);

        if (nextSpawnPoint != null)
        {
            // Convert local position to world position
            patch.transform.position = nextSpawnPoint;
        }

        activeEnvironmentPatches.Add(patch);

        if (activeEnvironmentPatches.Count == EnvironmentPoolSize && !turnedPatchSpawned)
        {
            SpawnTurnedEnvironmentPatch();
        }

        return patch;
    }

    private void SpawnTurnedEnvironmentPatch()
    {
        if (TurnedEnvironmentPatchPrefab == null)
        {
            Debug.Log("TurnedEnvironmentPatchPrefab is not assigned.");
            return;
        }

        GameObject lastPatch = activeEnvironmentPatches[activeEnvironmentPatches.Count - 1];

        // Get the transform from Turned Patch where new patches should spawn
        Transform turnedPatchTransform = lastPatch.GetComponent<EnvironmentPatch>().TurnedEnviornmentPatch.transform;

        GameObject turnedPatch = Instantiate(TurnedEnvironmentPatchPrefab, lastPatch.transform);

        turnedPatch.transform.localPosition = turnedPatchTransform.localPosition;
        turnedPatch.SetActive(true);

        activeEnvironmentPatches.Add(turnedPatch);

        // **Set nextSpawnPoint to the position for future patches**
        nextSpawnPoint = turnedPatchTransform.position;

        turnedPatchSpawned = true;
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
