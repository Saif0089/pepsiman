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
    int spawned;
    public float correction;
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
    public void checkSpawn()
    {
       // if (spawned > initialPatchCount)
        {
            lastSpawnZ += correction; // Move next patch forward

            GameObject patch = ObjectPooler.Instance.GetActiveEnvironmentPatch();
            patch.GetComponent<EnvironmentPatch>().SetPosition(new Vector3(0, 0, lastSpawnZ));
        }
    }
    private void SpawnEnvironmentPatch()
    {
        spawned++;

        if (spawned <= initialPatchCount)
        {
            GameObject patch = ObjectPooler.Instance.GetActiveEnvironmentPatch();
            patch.GetComponent<EnvironmentPatch>().SetPosition(new Vector3(0, 0, lastSpawnZ));
           
            if(spawned<initialPatchCount)
            lastSpawnZ += patchLength; // Move next patch forward
        }
    }
    
    public void StopSpawning(bool state)
    {
        canSpawn = state;
    }
}
