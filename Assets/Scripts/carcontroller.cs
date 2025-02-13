using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carcontroller : MonoBehaviour
{
    public float moveTime=5;
    public List<Transform> movePoints;

    //public Rigidbody rb;
   
    int curr = 0;

    bool reached = false;
    // Start is called before the first frame update
    void Start()
    {
        resetMe();
    }

    public void resetMe()
    {
        curr = 0;
        transform.position = movePoints[curr].position;
        curr++;
        reached = false;
    }
    // Update is called once per frame
    void Update()
    {
       
    }
    private void FixedUpdate()
    {
        if (reached == false)
        {
              transform.position = Vector3.MoveTowards(transform.position, movePoints[curr].position, moveTime*Time.deltaTime);
          //  rb.MovePosition(movePoints[curr].position * moveTime * Time.fixedDeltaTime);
            transform.forward = (movePoints[curr].position - transform.position).normalized;
            if (Vector3.Distance(transform.position, movePoints[curr].position) <= 0.1f)
            {
                curr++;

                if (curr >= movePoints.Count)
                {
                    reached = true;
                }
            }
        }
        else
        {
            if (transform.position.z <= 3)
            {
                resetMe();
            }
        }

    }
}
