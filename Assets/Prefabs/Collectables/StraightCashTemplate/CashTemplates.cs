using System;
using UnityEngine;
using System.Collections.Generic;

public class CashTemplates : MonoBehaviour
{
        public LayerMask targetLayer;
        public List<GameObject> cashTemplates;

        private void OnEnable()
        {
            foreach (GameObject cashTemplate in cashTemplates)
            {
                cashTemplate.SetActive(true);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & targetLayer) != 0)
            {
                gameObject.SetActive(false);
                Debug.Log("Cash templates Destroyed");
            }
        }
}