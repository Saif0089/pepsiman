using UnityEngine;

public class CashTemplates : MonoBehaviour
{
        public LayerMask targetLayer;
        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & targetLayer) != 0)
            {
                Destroy(gameObject);
                Debug.Log("Cash templates Destroyed");
            }
        }
}