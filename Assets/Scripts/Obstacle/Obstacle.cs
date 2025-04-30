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
    public GameObject ParentCar;
    public bool OffOnHit = true;
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
    public void ToggleCarOnHit()
    {
        if (OffOnHit)
        {
            ParentCar.SetActive(false);
            Invoke(nameof(OnCarAgain),5f);
        }
    }
    void OnCarAgain()
    {
        ParentCar.SetActive(true);
    }
    public void StopMovement(bool state)
    {
        isMoving = state;
    }
    
    public void OnTrgiggered()
    {
        Invoke(nameof(DelayDisable), 0.4f);
    }

    private void DelayDisable()
    {
        gameObject.SetActive(false);
    }
}