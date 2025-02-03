using System;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;
    public int initialPatchCount = 5;
    public float patchLength = 50f;
    private float lastSpawnZ = 0f;
    public Camera mainCamera;
    private bool canSpawn = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Spawn initial patches
        for (int i = 0; i < initialPatchCount; i++)
        {
            SpawnEnvironmentPatch();
        }
        
        //mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!canSpawn) return;
        
        if (lastSpawnZ - mainCamera.transform.position.z < 400f) // Adjust distance threshold
        {
            SpawnEnvironmentPatch();
        }
    }

    private void SpawnEnvironmentPatch()
    {
        GameObject patch = ObjectPooler.Instance.GetEnvironmentPatch();
        patch.GetComponent<EnvironmentPatch>().SetPosition(new Vector3(0, 0, lastSpawnZ));
        lastSpawnZ += patchLength; // Move next patch forward
    }
    
    public void StopSpawning(bool state)
    {
        canSpawn = state;
    }
}
