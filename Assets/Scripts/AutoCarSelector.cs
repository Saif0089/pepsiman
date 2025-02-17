using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCarSelector : MonoBehaviour
{
    public GameObject[] cars; 
    // Start is called before the first frame update
    void Start()
    {
        int rand=Random.Range(0,cars.Length);

        for(int i=0;i<cars.Length;i++)
        {
            cars[i].SetActive(rand == i);
        }
        
    }
    private void OnEnable()
    {
        int rand = Random.Range(0, cars.Length);

        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(rand == i);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
