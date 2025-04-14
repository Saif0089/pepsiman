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
                PlayerController.instance.SkateBoard.SetActive(true);
                PlayerController.instance.ToggleMagnet();
                PlayerController.instance.animator.SetBool("Skate",true);
                gameObject.SetActive(false);
                Invoke(nameof(OnSkateAgain),3f);
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
