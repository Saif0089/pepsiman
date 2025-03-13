using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnedPatchEnv : MonoBehaviour
{
   public Transform PatchPoint;

   public Transform SchoolPoint;
   
   public GameObject Barrier;
   
   public List<GameObject> AllCashTemplates;

   public List<GameObject> AllPuddles;

   public void TurnOnAllCash()
   {
      foreach (GameObject cash in AllCashTemplates)
      {
         cash.SetActive(true);
      }
   }
}
