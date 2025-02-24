using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPatch : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the patch moves
    private bool isMoving = true;

    public List<GameObject> gameObjects = new List<GameObject>();

    public GameObject crossroadpatch;
    public GameObject crossroadreplacementpatch;
    public GameObject[] NPCinMid;
    private void Start()
    {


        int rand1 = Random.Range(0, 100);
        if (rand1 <= 0)
        {

            crossroadreplacementpatch.SetActive(true);
            crossroadpatch.SetActive(false);
        }
        else
        {
            crossroadpatch.SetActive(true);
            crossroadreplacementpatch.SetActive(false);
            int NPCRandom = Random.Range(0, NPCinMid.Length);

            for (int i = 0; i < NPCinMid.Length; i++)
            {

                NPCinMid[i].SetActive(i == NPCRandom);



            }
        }


        for (int i = 0; i < 2; i++)
        {

            int rand = Random.Range(0,gameObjects.Count);

               gameObjects.Remove( gameObjects[rand]);
            
        }
        
       foreach (GameObject obj in gameObjects) { Destroy(obj.gameObject); }


        

    }
    private void OnEnable()
    {

        int rand1 = Random.Range(0, 100);
        if (rand1 <= 50)
        {

            crossroadreplacementpatch.SetActive(true);
            crossroadpatch.SetActive(false);
        }
        else
        {
            crossroadpatch.SetActive(true);
            crossroadreplacementpatch.SetActive(false);
            int NPCRandom = Random.Range(0, NPCinMid.Length);

            for (int i = 0; i < NPCinMid.Length; i++)
            {

                NPCinMid[i].SetActive(i == NPCRandom);



            }
        }

    }
    private void Update()
    {
        if (isMoving)
        {
            float movementSpeed = PlayerController.instance.getCurrSpeed();

            if (PlayerController.instance.BoostEnabled)
                movementSpeed += PlayerController.instance.moveForwardBoostSpeed;

            transform.Translate(Vector3.back * (movementSpeed * Time.deltaTime));

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