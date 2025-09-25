using System;
using UnityEngine;

public class FieldGenerator : MonoBehaviour, IGameSystem
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GridManager gridManager;

    public event Action OnReady;

  
    public void Initialize()
    {
        GenerateField();
        OnReady?.Invoke(); // Avisamos que ya terminamos
    }

    void GenerateField()
    {
        foreach (var kvp in gridManager.Grid)
        {
            Vector2Int cords = kvp.Key;
            Vector3 spawnPos = gridManager.GetPositionFromCoordinates(cords);
            Instantiate(tilePrefab, spawnPos, Quaternion.identity);
        }
    }

}
