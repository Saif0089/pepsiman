using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refresh : MonoBehaviour
{
   public float Timer;

   private void Update()
   {
      Timer += Time.time;

      if (Timer >= 5f)
      {
         Debug.Log("Refreshed");
      }
   }
}
