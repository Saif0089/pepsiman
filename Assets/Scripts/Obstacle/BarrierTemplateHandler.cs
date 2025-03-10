using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarrierTemplateHandler : MonoBehaviour
{
    public List<GameObject> AllBarriers;
    private void OnEnable()
    {
        OnRandomBarriers();
    }
    public void OnRandomBarriers()
    {
        OffAllBarriers();
        
        int rand = Random.Range(0, AllBarriers.Count);
        AllBarriers[rand].SetActive(true);
    }
    public void OffAllBarriers()
    {
        foreach (GameObject barrier in AllBarriers)
        {
            barrier.SetActive(false);
        }
    }
    private void OnDisable()
    {
        OffAllBarriers();
    }
}
