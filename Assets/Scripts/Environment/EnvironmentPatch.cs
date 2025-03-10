using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPatch : MonoBehaviour
{
    private bool isMoving = true;
    public bool canOffThis;

    public List<GameObject> gameObjects = new List<GameObject>();
    public List<GameObject> AllCashTemplates;
    public List<BarrierTemplateHandler> AllBarrierTemplates;

    public GameObject crossroadpatch;
    public GameObject TurnedPatch;
    public Transform NextPoint;
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

        // Randomly remove 2 objects from the gameObjects list
        for (int i = 0; i < 2 && gameObjects.Count > 0; i++)
        {
            int rand = Random.Range(0, gameObjects.Count);
            Destroy(gameObjects[rand]);
            gameObjects.RemoveAt(rand); // Corrected: Use RemoveAt to avoid index errors
        }
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
        // ShufflePositions(); // Shuffle when re-enabled
    }
    public void ShufflePositions()
    {
        if (AllCashTemplates.Count <= 1) return;

        // Get all current positions
        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject cashTemplate in AllCashTemplates)
        {
            positions.Add(cashTemplate.transform.position);
        }

        // Shuffle the positions to avoid duplicates
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (positions[i], positions[randomIndex]) = (positions[randomIndex], positions[i]);
        }

        // Select a random subset (2-3 items) to actually move
        int swaps = Random.Range(2, 4);
        HashSet<int> selectedIndexes = new HashSet<int>();

        while (selectedIndexes.Count < swaps)
        {
            selectedIndexes.Add(Random.Range(0, AllCashTemplates.Count));
        }

        int posIndex = 0;
        foreach (int index in selectedIndexes)
        {
            AllCashTemplates[index].transform.position = positions[posIndex];
            posIndex++;
        }
    }
    void Update()
    {
        if (isMoving)
        {
            float movementSpeed = PlayerController.instance.getCurrSpeed();

            if (PlayerController.instance.BoostEnabled)
                movementSpeed += PlayerController.instance.moveForwardBoostSpeed;

            transform.position += Camera.main.transform.forward * (-movementSpeed * Time.deltaTime);

            // Check the distance from the camera
            float distanceBehindCamera = Camera.main.transform.position.z - transform.position.z;
            float deactivateDistance = 190f; // Adjust this threshold as needed

            if (canOffThis && distanceBehindCamera > deactivateDistance)
            {
                gameObject.SetActive(false);
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
