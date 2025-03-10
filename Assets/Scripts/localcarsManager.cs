using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class localcarsManager : MonoBehaviour
{
    public GameObject carA;
    public GameObject carB;

    public Transform StartA;
    public Transform StartB;
    public Transform EndA;
    public Transform EndB;
    public float MoveTime=8f;

    bool movenow = false;
    public bool UpdateDirection;


    public bool canmove=true;
    public bool canSet = true;

    public Animator animator;
    public GameObject[] characters;
    public Avatar[] avatars;
    // Start is called before the first frame update
    void Start()
    {

   
      MoveNow();
    }
    public void SetCharacter()
    {
        if (canSet)
        {
            int rand = Random.Range(0, characters.Length-1);

            animator.avatar = avatars[rand];

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].SetActive(i == rand);
            }
        }
    }
    public void MoveNow()
    {
        SetCharacter();
        if (canmove)
        {
            //  carA.transform.DOKill();
            carA.transform.localPosition = StartA.localPosition;
            //  carA.transform.(EndA.position, MoveTime);

            //  carB.transform.DOKill();
            carB.transform.localPosition = StartB.localPosition;
            //   carB.transform.DOMove(EndB.position, MoveTime);

            movenow = true;
            stopB = false;
            stopA = false;
        }
    }
    bool stopA=false;
    bool stopB=false;
    // Update is called once per frame
    void Update()
    {
        if (canmove)
        {
            if (movenow)
            {
                if (!stopA)
                    if (Vector3.Distance(carA.transform.position, EndA.transform.position) > 0.1f)
                    {
                        carA.transform.localPosition = Vector3.MoveTowards(carA.transform.localPosition, EndA.transform.localPosition, (float)MoveTime * Time.deltaTime);
                    }
                    else { stopA = true; }

                if (!stopB)
                    if (Vector3.Distance(carB.transform.position, EndB.transform.position) > 0.1f)
                    {
                        carB.transform.localPosition = Vector3.MoveTowards(carB.transform.localPosition, EndB.transform.localPosition, (float)MoveTime * Time.deltaTime);
                    }
                    else { stopB = true; }

                if (stopA && stopB)
                {
                    movenow = false;
                    MoveNow();
                }

            }

            if (UpdateDirection)
            {
                carA.transform.forward = (EndA.transform.position - carA.transform.position).normalized;
                carB.transform.forward = (EndB.transform.position - carB.transform.position).normalized;
            }

        }
        
    }
}
