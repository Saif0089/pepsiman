using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the obstacle moves towards the player
    private bool isMoving = true;
    public bool canmove = true;
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