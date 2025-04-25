using UnityEngine;
using DG.Tweening;
public class PuddleHandler : MonoBehaviour
{
    [SerializeField] LayerMask TargetLayer;
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & TargetLayer) != 0)
        {
            Stumble();
        }
    }
    void Stumble()
    {
        PlayerController.instance.canMovement = false;
        PlayerController.instance.animator.SetTrigger("Stumble");
        // PlayerController.instance.moveDirection.x = 0f;
        PlayerController.instance.moveForwardSpeed = 7f;
    }

}
