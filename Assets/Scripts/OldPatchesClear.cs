using UnityEngine;

public class OldPatchesClear : MonoBehaviour
{
    public LayerMask layerMask;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            DestroyOldPatches();
        }
    }
    void DestroyOldPatches()
    {
        if(!ObjectPooler.Instance.CanDestroy)
            return;
        
        foreach (GameObject patch in ObjectPooler.Instance.activeEnvironmentPatches)
        {
            patch.SetActive(false);
            Destroy(patch);
        }
        ObjectPooler.Instance.activeEnvironmentPatches.Clear();
        ObjectPooler.Instance.activeEnvironmentPatches.AddRange(ObjectPooler.Instance.newPatches);
        ObjectPooler.Instance.newPatches.Clear();

        Debug.Log("Old Patches cleared");
        
        ObjectPooler.Instance.CanDestroy = false;
    }
}