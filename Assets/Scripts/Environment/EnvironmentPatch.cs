using UnityEngine;

public class EnvironmentPatch : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the patch moves
    private bool isMoving = true;

    private void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.back * (moveSpeed * Time.deltaTime));

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