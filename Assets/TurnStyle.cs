using System;
using UnityEngine;
using DG.Tweening;

public class TurnStyle : MonoBehaviour
{
   public LayerMask TargetLayer;

   public float RotationAngle;

   public float RotationSpeed;
   void OnTriggerEnter(Collider other)
   {
      if (((1 << other.gameObject.layer) & TargetLayer) != 0)
      {
         RotateEnvPatch();
      }
   }
   void RotateEnvPatch()
   {
      ObjectPooler.Instance.EnvironmentHolder.transform.DORotate(new Vector3(0, RotationAngle,0), RotationSpeed , RotateMode.Fast);

      DOTween.Kill(this);
   }
}
