using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuddleManager : MonoBehaviour
{
   public List<GameObject> AllPuddles;
   private void OnEnable()
   {
      OffAllPuddles();
      OnRandomPuddle();
   }
   public void OffAllPuddles()
   {
      for (int i = 0; i < AllPuddles.Count; i++)
      {
         AllPuddles[i].SetActive(false);
      }
   }
   public void OnRandomPuddle()
   {
      OffAllPuddles();
      int randomPuddle = Random.Range(0, AllPuddles.Count);
      AllPuddles[randomPuddle].SetActive(true);
   }
}
