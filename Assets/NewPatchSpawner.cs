using System;
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

        bool isFirstPatch = true; // Flag to track the first patch

        foreach (GameObject patch in ObjectPooler.Instance.activeEnvironmentPatches)
        {
            GameObject newPatch = Instantiate(patch);
            newPatch.transform.SetParent(envHolder);

            if (isFirstPatch)
            {
                newPatch.transform.position = ObjectPooler.Instance.NextPoint.position;
                newPatch.transform.rotation = ObjectPooler.Instance.NextPoint.rotation;
                isFirstPatch = false; // Only set position for the first patch
            }

            ObjectPooler.Instance.newPatches.Add(newPatch);

            EnvironmentPatch envPatch = newPatch.GetComponent<EnvironmentPatch>();
            if (envPatch != null && envPatch.TurnedPatch != null)
            {
                TurnedPatchEnv turnedPatch = envPatch.TurnedPatch.GetComponent<TurnedPatchEnv>();
                if (turnedPatch != null)
                {
                    ObjectPooler.Instance.NextPoint = turnedPatch.PatchPoint;
                }
            }
        }
        ObjectPooler.Instance.CanDestroy = true;
        SetPos();
    }
    
    public void SetPos()
    {
        for (int i = 1; i < ObjectPooler.Instance.newPatches.Count; i++)
        {
            // Get the previous patch's NextPoint world position
            Vector3 worldPos = ObjectPooler.Instance.newPatches[i - 1].GetComponent<EnvironmentPatch>().NextPoint.position;

            // Set the position of the current patch
            ObjectPooler.Instance.newPatches[i].transform.position = worldPos;

            // Match the rotation to the previous patch
            ObjectPooler.Instance.newPatches[i].transform.rotation = ObjectPooler.Instance.newPatches[i - 1].transform.rotation;
        }
    }



}