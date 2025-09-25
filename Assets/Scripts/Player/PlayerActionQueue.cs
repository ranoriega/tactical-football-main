using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class ShotIntent
{
    public Transform shooter;
    public Transform goalCenter;

    public Vector3 offset;
    
}
public class PlayerActionQueue : MonoBehaviour
{

    public Queue<PlayerAction> queuedAction = new Queue<PlayerAction>();
     public Dictionary<string, string> passQueue = new Dictionary<string, string>(); // 👈 aquí está
    public Queue<Transform> passQueueTwo = new Queue<Transform>();
    public List<Node> coordinates1 = new List<Node>();

    [SerializeField] public string playerID;
     public ShotIntent pendingShot;

    [System.Obsolete]
    private void Awake()
    {

    }

  
    public void QueueAction(PlayerAction action)
    {
         queuedAction.Enqueue(action); // ✅ agrega al final de la cola
    }
     // Ejecutar la siguiente acción de la cola
    public void ExecuteNextAction(Action onComplete)
    {
        if (queuedAction.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        PlayerAction action = queuedAction.Dequeue();

        if (action is MoveAction move)
        {
          
            if (move != null)
                move.ExecuteWithCallback(() =>
                {
                    // Al terminar movimiento, seguimos con la siguiente acción
                    ExecuteNextAction(onComplete);
                });
        }
        else
        {
            // Para PassAction o ShootAction
            action.Execute();
            ExecuteNextAction(onComplete); // ejecuta la siguiente acción inmediatamente
        }
    }

    public void ClearAction()
    {
        queuedAction.Clear();
    }

    public void SetMoveTarget(List<Node> coordinates)
    {
        coordinates1?.Clear();
        coordinates1 = new List<Node>(coordinates);

       // string pathStr = string.Join(" -> ", coordinates1.ConvertAll(n => n.cords.ToString()));
      //  MyDebug.Log($"Camino guardado: {pathStr}");


    }

  
    public void RegisterShot(Transform shooter, Vector3 trayectoryShot)
    {
        if (shooter == null)
        {
            MyDebug.LogWarning("RegisterShot: shooter es null, no se guarda el tiro.");
            return;
        }
        int shooterID = shooter.GetComponent<PlayerTeam>().teamID;

        pendingShot = new ShotIntent
        {
            shooter = shooter,
            goalCenter = GetOpponentGoal(shooterID),
            offset = trayectoryShot

        };

       
    
    }

      public Transform GetOpponentGoal(int shooterID)
    {
        Goal[] allGoals = FindObjectsByType<Goal>(FindObjectsSortMode.None);
        foreach (Goal goal in allGoals)
        {
            if (goal.teamID != shooterID) // Buscar portería del oponente
            {
                
                  Transform  goalCenter = GoalSpawner.Instance.GetOpponentGoalCenter(shooterID);
                if (goalCenter == null)
                {
                    MyDebug.Log("no se encontro centro ");
                    return null;
                }
                return goalCenter.transform;
            }
        }
        return null;
    }

    public void QueuePass(Transform receiver)
    {
        if (receiver == null) return;

        // Guardar la acción de pase en la cola
        passQueueTwo.Enqueue(receiver.transform);

         MyDebug.Log($"{playerID} programó un pase hacia {receiver.GetComponent<PlayerActionQueue>().playerID}");
    }



    public List<Node> getMoveTarget()
    {
        return coordinates1;
    }

}
