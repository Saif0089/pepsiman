using DG.Tweening;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the obstacle moves towards the player
    private bool isMoving = true;
    public int myid;
    private void OnEnable()
    {
        // Start floating animation when the collectable is activated
        AnimateFloating();
    }
    
    private void Update()
    {
        if (isMoving)
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
            CollectableSpawner.Instance.AddUiEffectCollected(gameObject.transform.position,myid);
            gameObject.SetActive(false);
        }
    }
    
    private void AnimateFloating()
    {
        // Make the collectable float up and down smoothly
        transform.DOKill(); // Stop any previous animations to prevent stacking
        transform.DOMoveY(transform.position.y + 0.5f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Infinite up/down animation
    }
}
