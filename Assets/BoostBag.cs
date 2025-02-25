using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostBag : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float moveSpeed = 1f; // Speed of up-and-down movement
    public float moveHeight = 0.5f; // Height of the movement

    private Vector3 startPos;
    public ParticleSystem PickUpParticles;
    
    private void Start()
    {
        startPos = transform.position; // Store the initial position
    }

    private void OnDisable()
    {
        FindObjectOfType<ObjectPooler>().bag.Clear();
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        RotateBag();
        MoveUpDown();
    }

    private void RotateBag()
    {
        transform.rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
    }
    private void MoveUpDown()
    {
        float newY = startPos.y + Mathf.PingPong(Time.time * moveSpeed, moveHeight) - (moveHeight / 2);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Audiomanager.instance.PlaySfx_bag();
            PickUpParticles.Play();
            Invoke(nameof(GiveBoost), 0.1f);
        }
    }
    void GiveBoost()
    {
        PlayerController.instance.BoostEnabled = true;
        Destroy(gameObject);
    }
}