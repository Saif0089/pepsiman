using System;
using DG.Tweening;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving = true;
    public int myid;
    public bool canMove = true;
    public bool canAnimate = true;

    LayerMask targetLayer;

    Vector3 initialPosition;
    Vector3 initialPosition_2;

    private void Awake()
    {
        targetLayer = LayerMask.GetMask("Col");
        initialPosition = transform.localPosition;
    }
    private void OnEnable()
    {
        SetDefaultPos();
        
        initialPosition_2 = transform.position;
        Breathing();
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
    void Breathing()
    {
        transform.DOMoveY(initialPosition_2.y + 0.25f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    void SetDefaultPos()
    {
        transform.localPosition = initialPosition;
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
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        transform.DOMove(PlayerController.instance.CashPoint.position, 0.1f)
            .OnComplete(() =>
            {
                SetDefaultPos();
                gameObject.SetActive(false);
            });
    }
}