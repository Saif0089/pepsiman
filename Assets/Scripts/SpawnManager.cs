using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance => _instance;

    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // Stores (x, z) positions

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }
 
    public bool IsPositionOccupied(float x, float z)
    {
        return occupiedPositions.Contains(new Vector2(x, z));
    }

    public void MarkPositionOccupied(float x, float z)
    {
        occupiedPositions.Add(new Vector2(x, z));
    }
}