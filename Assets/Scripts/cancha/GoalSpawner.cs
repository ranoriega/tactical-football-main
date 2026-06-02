using System;
using UnityEngine;

public class GoalSpawner : MonoBehaviour, IGameSystem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
      [SerializeField] GameObject goalPrefab;
    [SerializeField] GridManager gridManager;
      private Goal[] goals; // Los dos arcos
     public static GoalSpawner Instance;
    public event Action OnReady;


 private void Awake()
    {
        // Singleton para que exista solo una instancia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
  void SpawnGoals()
  {
    goals = new Goal[2];

    float yOffset = 0.12f; // altura del arco
    float rotationY = -180.2f; // ángulo correcto según tu prefab

    float centerX = gridManager.gridSize.x / 2f;
    float firstRowZ = gridManager.GetPositionFromCoordinates(new Vector2Int(0, 0)).z;
    float lastRowZ = gridManager.GetPositionFromCoordinates(new Vector2Int(0, gridManager.gridSize.y - 1)).z;

    Vector3 posA = new Vector3(centerX, yOffset, firstRowZ - 0.67f);
    Vector3 posB = new Vector3(centerX, yOffset, lastRowZ + 0.13f);



    GameObject goalA = Instantiate(goalPrefab, posA, Quaternion.Euler(357.321167f,276.075745f,356.754944f));
    GameObject goalB = Instantiate(goalPrefab, posB, Quaternion.Euler(0, 271.390015f, 181.210007f));
    

  goals[0] = goalA.GetComponent<Goal>();
    goals[0].teamID = 1;
  goals[1] = goalB.GetComponent<Goal>();
  goals[1].teamID = 2;

    
  }
    public Transform GetOpponentGoalCenter(int teamID)
    {
        // Devuelve el arco contrario
       if (teamID == 1)
       {
        return goals[1].GetComponent<Goal>().goalCenter;
        
       }else if(teamID ==2)       
       {
         return goals[0].GetComponent<Goal>().goalCenter;

    }
    else
    {
      MyDebug.Log("no hay teamid");
    }
        return null;
    }
  public void Initialize()
  {
    
           SpawnGoals();
           OnReady?.Invoke();
    }
}
