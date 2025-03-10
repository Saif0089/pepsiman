using System;
using System.Linq;
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

        bool isFirstPatch = true;

        foreach (GameObject patch in ObjectPooler.Instance.activeEnvironmentPatches)
        {
            GameObject newPatch = Instantiate(patch);
            newPatch.transform.SetParent(envHolder);

            if (isFirstPatch)
            {
                newPatch.transform.position = ObjectPooler.Instance.NextPoint.position;
                newPatch.transform.rotation = ObjectPooler.Instance.NextPoint.rotation;
                isFirstPatch = false;
            }

            ObjectPooler.Instance.newPatches.Add(newPatch);
        }
        
        SpawnNextTurnedPatch();
        ObjectPooler.Instance.NextPoint = ObjectPooler.Instance.ActivedTuredPatch.GetComponent<TurnedPatchEnv>().PatchPoint;
        ObjectPooler.Instance.CanDestroy = true;
        SetPos();
    }

    public void SpawnNextTurnedPatch()
    {
        if (ObjectPooler.Instance.NextPatchWillBe == ObjectPooler.NextPatchToSpawn.LeftTurn)
        {
            ObjectPooler.Instance.TurnedPatchParent = ObjectPooler.Instance.newPatches.Last().GetComponent<EnvironmentPatch>().TurnedPatch;

            ObjectPooler.Instance.TurnedPatchParent.GetComponent<TurnedPatchesManager>().ActivateLeft();

            ObjectPooler.Instance.NextPatchWillBe = ObjectPooler.NextPatchToSpawn.RightTurn;

            foreach (GameObject patch in ObjectPooler.Instance.newPatches)
            {
                patch.SetActive(true);
            }
        }
        else if (ObjectPooler.Instance.NextPatchWillBe == ObjectPooler.NextPatchToSpawn.RightTurn)
        {
            ObjectPooler.Instance.TurnedPatchParent = ObjectPooler.Instance.newPatches.Last().GetComponent<EnvironmentPatch>().TurnedPatch;

            ObjectPooler.Instance.TurnedPatchParent.GetComponent<TurnedPatchesManager>().ActivateRight();

            ObjectPooler.Instance.NextPatchWillBe = ObjectPooler.NextPatchToSpawn.LeftTurn;
            
            foreach (GameObject patch in ObjectPooler.Instance.newPatches)
            {
                patch.SetActive(true);
            }
        }
    }

    public void SetPos()
    {
        for (int i = 1; i < ObjectPooler.Instance.newPatches.Count; i++)
        {
            Vector3 worldPos = ObjectPooler.Instance.newPatches[i - 1].GetComponent<EnvironmentPatch>().NextPoint.position;

            ObjectPooler.Instance.newPatches[i].transform.position = worldPos;

            ObjectPooler.Instance.newPatches[i].transform.rotation = ObjectPooler.Instance.newPatches[i - 1].transform.rotation;
        }
    }
}