using System;
using UnityEngine;
using DG.Tweening;

public class TurnStyle : MonoBehaviour
{
    public LayerMask TargetLayer;
    public float RotationSpeed = 1f;
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & TargetLayer) != 0)
        {
            RotateEnvPatch();
        }
    }
    void RotateEnvPatch()
    {
        Transform envHolder = ObjectPooler.Instance.EnvironmentHolder.transform;
        float newYRotation = envHolder.eulerAngles.y - 45f;

        envHolder.DORotate(new Vector3(0, newYRotation, 0), RotationSpeed, RotateMode.Fast)
            .OnComplete(() => 
            {
                if (Mathf.Abs(newYRotation) >= 360f)
                {
                    envHolder.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                RemoveOldPatches();

                for (int i = 0; i < ObjectPooler.Instance.EnvironmentPoolSize; i++)
                {
                    ObjectPooler.Instance.GetActiveEnvironmentPatch();
                }
            });
    }
    void RemoveOldPatches()
    {
        ObjectPooler pooler = ObjectPooler.Instance;
        if (pooler.activeEnvironmentPatches.Count > 0)
        {
            GameObject firstPatch = pooler.activeEnvironmentPatches[0]; // Oldest patch

            if (firstPatch.transform.position.z < pooler.EnvironmentHolder.transform.position.z - 10f) // Adjust condition as needed
            {
                pooler.ReturnActiveEnvironmentPatch(firstPatch);
            }
        }
    }


}