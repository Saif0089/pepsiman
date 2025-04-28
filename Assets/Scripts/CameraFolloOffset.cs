using UnityEngine;

public class CameraFollowOffset : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    float offsetX = 0f;
    public Vector2 offsetXZ = Vector2.zero; // Offset for X and Z
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(
                target.position.x + offsetXZ.x,
                transform.position.y, // Keep Y unchanged
                target.position.z + offsetXZ.y
            );

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        }
    }

}