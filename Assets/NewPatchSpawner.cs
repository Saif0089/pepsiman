using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPatchSpawner : MonoBehaviour
{
    public LayerMask TargetLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & TargetLayer) != 0)
        {
            SpawnNewPatches();
        }
    }
    void SpawnNewPatches()
    {
        Transform envHolder = ObjectPooler.Instance.EnvironmentHolder.transform;

        Debug.Log($"Current NextPoint: {ObjectPooler.Instance.NextPoint}");

        float xOffset = 195f; // Set your desired offset value
        float accumulatedOffset = 0f; // Track the cumulative offset

        foreach (GameObject patch in ObjectPooler.Instance.activeEnvironmentPatches)
        {
            GameObject newPatch = Instantiate(patch);
            newPatch.transform.SetParent(envHolder);

            // Apply offset to x position
            Vector3 newPosition = ObjectPooler.Instance.NextPoint.position;
            newPosition.x += accumulatedOffset; // Add accumulated offset to x position
            newPatch.transform.position = newPosition;
            newPatch.transform.rotation = ObjectPooler.Instance.NextPoint.rotation;

            ObjectPooler.Instance.newPatches.Add(newPatch);

            EnvironmentPatch envPatch = newPatch.GetComponent<EnvironmentPatch>();
            if (envPatch != null && envPatch.TurnedPatch != null)
            {
                TurnedPatchEnv turnedPatch = envPatch.TurnedPatch.GetComponent<TurnedPatchEnv>();
                if (turnedPatch != null)
                {
                    ObjectPooler.Instance.NextPoint = turnedPatch.PatchPoint;
                    Debug.Log($"Updated NextPoint to: {ObjectPooler.Instance.NextPoint}");
                }
            }

            accumulatedOffset += xOffset; // Increase offset for next patch
        }

        ObjectPooler.Instance.CanDestroy = true;
        SetPositions();
    }

    void SetPositions()
    {
        var patches = ObjectPooler.Instance.newPatches; // Store reference

        if (patches == null || patches.Count < 2)
        {
            Debug.LogWarning("newPatches list is null or has less than two elements.");
            return;
        }

        for (int i = patches.Count - 1; i > 0; i--)
        {
            if (patches[i] != null && patches[i - 1] != null)
            {
                float previousZ = patches[i - 1].transform.localPosition.z;
                Vector3 position = patches[i].transform.localPosition;
                position.z = previousZ;
                patches[i].transform.localPosition = position;

                Debug.Log($"Updated Patch {i}: New Z = {previousZ}");
            }
            else
            {
                Debug.LogWarning($"Patch at index {i} or {i - 1} is null.");
            }
        }
    }

}
