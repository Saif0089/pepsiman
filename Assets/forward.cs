using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forward : MonoBehaviour
{
    public float Speed;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Speed);
    }
}
