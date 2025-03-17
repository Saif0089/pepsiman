using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

public class EndingTrigger : MonoBehaviour
{
    public LayerMask TargetLayer;

    public GameObject VirtualCam, Dolly;

    public PlayableDirector EndTimeLine;

    private void OnEnable()
    {
        VirtualCam.SetActive(false);
        Dolly.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & TargetLayer) != 0)
        {
            GameManager.Instance.CharacterCam.SetActive(true);
            PlayerController.instance.canMovement = false;

            PlayerController.instance.transform.DOMoveX(0f, 0.2f);
            PlayerController.instance.transform.DORotate(Vector3.zero, 0.2f);
            
            PlayerController.instance.moveDirection.x = 0f;

            
            Invoke(nameof(TriggerEndingTimeline),0.2f);
        }
    }
    public void TriggerEndingTimeline()
    {
        VirtualCam.SetActive(true);
        EndTimeLine.Play();
    }


}
