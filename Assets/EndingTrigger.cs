using System;
using UnityEngine;
using UnityEngine.Playables;

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
            TriggerEndingTimeline();
        }
    }
    public void TriggerEndingTimeline()
    {
        VirtualCam.SetActive(true);
        VirtualCam.SetActive(true);
        EndTimeLine.Play();
    }
}
