using UnityEngine;
using DG.Tweening;

public class TurnStyle : MonoBehaviour
{
    public LayerMask TargetLayer;
    public float RotationSpeed = 1f;
    public bool IsLeftTurn;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & TargetLayer) != 0)
        {
            RotateEnvPatch();
        }
    }
    private void RotateEnvPatch()
    {
        Transform envHolder = ObjectPooler.Instance.EnvironmentHolder.transform;
        float newRotation = envHolder.eulerAngles.y + (IsLeftTurn ? 45f : -45f);

        envHolder.DORotate(new Vector3(0, newRotation, 0), RotationSpeed, RotateMode.Fast)
            .OnComplete(() =>
            {
                // Normalize rotation to avoid exceeding 360 degrees
                float currentY = envHolder.eulerAngles.y;
                if (currentY >= 360f || currentY <= -360f)
                {
                    envHolder.rotation = Quaternion.Euler(0f, currentY % 360f, 0f);
                }
            });
    }
}