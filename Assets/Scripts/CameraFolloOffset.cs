using System;
using UnityEngine;
using DG.Tweening;

public class CameraFollowOffset : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector2 offsetXZ = Vector2.zero; // Offset for X and Z

    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float strength = 0.5f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float randomness = 90f;

    public void Shake()
    {
        transform.DOShakePosition(duration, strength, vibrato, randomness)
            .SetEase(Ease.OutQuad);
    }

    void FixedUpdate()
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