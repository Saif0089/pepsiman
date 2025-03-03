using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving = true;
    public int myid;
    public bool canMove = true;
    public bool canAnimate = true;

    public LayerMask targetLayer;
    
    private float initialY;
    public float floatAmount = 0.5f;
    private float floatSpeed = 2f;
    private bool floatingUp = true;

    private void Awake()
    {
        targetLayer = LayerMask.GetMask("Runner");
    }

    private void OnEnable()
    {
        initialY = transform.position.y; // Store the initial Y position
    }

    private void Update()
    {
        if (isMoving && canMove)
        {
            transform.Translate(Vector3.back * (moveSpeed * Time.deltaTime));

            if (transform.position.z < -15f)
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
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            CollectableSpawner.Instance.AddUiEffectCollected(transform.position, myid);
            gameObject.SetActive(false);
        }
    }
}