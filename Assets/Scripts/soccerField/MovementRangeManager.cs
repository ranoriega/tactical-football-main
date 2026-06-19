using System.Collections.Generic;
using UnityEngine;

public class MovementRangeManager : MonoBehaviour
{
    public static MovementRangeManager Instance;

    public int moveRange = 5;

    GridManager gridManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);

        else Instance = this;

       gridManager = FindAnyObjectByType<GridManager>();
    }
    
    public void ShowRange(Transform unit)
    {
        ClearRange();

        Vector2Int origin =
            gridManager.GetCoordinatesFromPosition(unit.position);

        Pathfinding.Instance.GetReachableArea(origin, moveRange);
    }

    public void ClearRange()
    {
        foreach (var node in gridManager.Grid.Values)
        {
            node.inRange = false;
        }
       
    }
}
