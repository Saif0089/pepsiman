using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the obstacle moves towards the player
    private bool isMoving = true;
    public bool canmove = true;
    public bool spawnCollectables=true;
    public List<GameObject> cashtemplates;
    public List<Transform> positions;
    private void Start()
    {
        int rand=Random.Range(0,cashtemplates.Count-1);

        float spawnchance = Random.Range(0, 100);
        if(spawnchance<95 && spawnCollectables)
        if (cashtemplates.Count > 0)
        {
            var temp = Instantiate(cashtemplates[rand], positions[Random.Range(0, positions.Count - 1)]);
            temp.transform.localPosition = Vector3.zero;
        }
    }
    private void Update()
    {
        if (isMoving && canmove)
        {
            transform.Translate(Vector3.back * (moveSpeed * Time.deltaTime));

            // Deactivate obstacle if it goes off-screen
            if (transform.position.z < -15f) // Adjust based on your game's needs
            {
                gameObject.SetActive(false);
            }
        }
    }
    
    public void StopMovement(bool state)
    {
        isMoving = state;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke(nameof(DelayDisable), 0.4f);
        }
    }

    private void DelayDisable()
    {
        gameObject.SetActive(false);
    }
}