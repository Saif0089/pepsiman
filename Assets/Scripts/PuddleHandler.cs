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
        PlayerController.instance.isHurt = true;
        PlayerController.instance.moveForwardSpeed = 10f;
        Audiomanager.instance.Play_StumbleClip();
    }

}
