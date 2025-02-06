using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPatch : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the patch moves
    private bool isMoving = true;

    public List<GameObject> gameObjects = new List<GameObject>();
    private void Start()
    {
      
       for (int i = 0; i < 2; i++)
        {

            int rand = Random.Range(0,gameObjects.Count);

               gameObjects.Remove( gameObjects[rand]);
            
        }
        
       foreach (GameObject obj in gameObjects) { Destroy(obj.gameObject); }

    }
    private void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.back * (PlayerController.instance.getCurrSpeed() * Time.deltaTime));

            // Recycle patch if it goes out of view
            if (transform.position.z < -190f) // Adjust based on game needs
            {
                ObjectPooler.Instance.ReturnEnvironmentPatch(gameObject);
            }
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    
    public void StopMovement(bool state)
    {
        isMoving = state;
    }
}