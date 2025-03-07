using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnedPatchesManager : MonoBehaviour
{
    public GameObject RightTurn, LeftTurn;
    
    public void ActivateRight()
    {
        LeftTurn.SetActive(false);
        RightTurn.SetActive(true);
        ObjectPooler.Instance.ActivedTuredPatch = RightTurn;
        
    }
    public void ActivateLeft()
    {
        RightTurn.SetActive(false);
        LeftTurn.SetActive(true);
        ObjectPooler.Instance.ActivedTuredPatch = LeftTurn;
    }
}
