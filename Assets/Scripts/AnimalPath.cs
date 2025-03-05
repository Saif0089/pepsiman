using System;
using UnityEngine;

public class AnimalPath : MonoBehaviour
{
    [SerializeField] Transform Animal, Target;
    [SerializeField] float Speed;
    [SerializeField] GameObject[] AllAnimals;
    Vector3 startingPosition, targetPosition;
    GameObject ActivatedAnimal;
    bool hasStartedMoving = false;
    bool movingToTarget = true;
    private void Awake()
    {
        DisableAllAnimals();
    }
    void Start()
    {
        int randomAnimal = UnityEngine.Random.Range(0, AllAnimals.Length);
        AllAnimals[randomAnimal].SetActive(true);
        ActivatedAnimal = AllAnimals[randomAnimal];
        hasStartedMoving = false;
        movingToTarget = true; 
        startingPosition = Animal.localPosition;
        targetPosition = Target.localPosition;
    }

    void Update()
    {
        Animal.localPosition = Vector3.MoveTowards(Animal.localPosition, targetPosition, Speed * Time.deltaTime);

        if (!hasStartedMoving && Vector3.Distance(Animal.localPosition, startingPosition) > 0.01f)
        {
            Debug.Log("Animal started moving towards the target.");
            ActivatedAnimal.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); 
            hasStartedMoving = true;
        }

        if (Vector3.Distance(Animal.localPosition, targetPosition) < 0.01f)
        {
            Debug.Log("Animal reached the target, now returning to starting position.");
            
            movingToTarget = !movingToTarget; // Toggle movement direction

            ActivatedAnimal.transform.localRotation = movingToTarget ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);

            Vector3 temp = startingPosition;
            startingPosition = targetPosition;
            targetPosition = temp;
        }
    }
    void DisableAllAnimals()
    {
        for (int i = 0; i < AllAnimals.Length; i++)
        {
            AllAnimals[i].SetActive(false);
        }
    }
}
