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
            PlayerController.instance.moveDirection.x = 0f;

            PlayerController.instance.transform.DOMoveX(0f, 1f);
            PlayerController.instance.transform.DORotate(new Vector3(PlayerController.instance.transform.eulerAngles.x, 0f, PlayerController.instance.transform.eulerAngles.z), 1f);
            
            Invoke(nameof(TriggerEndingTimeline),0.2f);
        }
    }
    public void TriggerEndingTimeline()
    {
        VirtualCam.SetActive(true);
        EndTimeLine.Play();
    }


}
