using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowOffset : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    float offsetX = 0f;
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Lerp(transform.position.x, target.position.x + offsetX, Time.deltaTime * smoothSpeed);
            transform.position = newPosition;
        }
    }
}