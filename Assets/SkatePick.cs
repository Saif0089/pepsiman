using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkatePick : MonoBehaviour
{
    public LayerMask layerMask;
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            if (!PlayerController.instance.IsSkateBoardOn)
            {
                PlayerController.instance.IsSkateBoardOn = true;
                PlayerController.instance.particles.SkatePick.Play();
                Audiomanager.instance.PlaySkatePickClip();
                PlayerController.instance.PlayerCollider.center = new Vector3(0f, 0.8697391f, 0.1132071f);
                PlayerController.instance.PlayerCollider.size = new Vector3(1, 2.213472f, 1.00319f);
                PlayerController.instance.GroundCheckRayCastLenght = 0.35f;

                PlayerController.instance.SkateBoard.SetActive(true);
                PlayerController.instance.ToggleMagnet();
                PlayerController.instance.animator.SetBool("Skate", true);
                gameObject.SetActive(false);
                Invoke(nameof(OnSkateAgain), 3f);
            }
        }
    }

    void OnSkateAgain()
    {
        gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * 150f);
    }
}