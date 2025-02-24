using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostBag : MonoBehaviour
{
    private bool isMoving = true;
    public bool canmove = true;

    private void OnDisable()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        if (isMoving && canmove)
        {
            float movementSpeed = PlayerController.instance.moveForwardSpeed;

            if (PlayerController.instance.BoostEnabled)
                movementSpeed += PlayerController.instance.moveForwardBoostSpeed;

            transform.Translate(Vector3.back * (movementSpeed * Time.deltaTime));

            if (transform.position.z < -15f) 
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController.instance.BoostEnabled = true;
            Destroy(gameObject);
        }
    }
    
    public void StopBag(bool state)
    {
        isMoving = state;
    }
}
