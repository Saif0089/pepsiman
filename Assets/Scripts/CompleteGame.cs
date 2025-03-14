using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteGame : MonoBehaviour
{
    public LayerMask mask;
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & mask) != 0)
        {
            LevelComplete();
        }
    }
    void LevelComplete()
    {
        GameManager.Instance.WinGame();
    }
}
