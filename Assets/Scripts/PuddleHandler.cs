using UnityEngine;
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
        if(PlayerController.instance.isHurt || PlayerController.instance.isStumble)
            return;
        
        PlayerController.instance.particles.Stumble.Play();
        PlayerController.instance.isStumble = true;
        PlayerController.instance.canMovement = false;
        PlayerController.instance.animator.SetBool("Slide",false);
        PlayerController.instance.animator.SetTrigger("Stumble");
        PlayerController.instance.moveForwardSpeed = 10f;
        Audiomanager.instance.Play_StumbleClip();
        
        if (PlayerController.instance.IsSkateBoardOn)
        {
            Audiomanager.instance.SkateBoard_Source.mute = true;
        }
    }

}
