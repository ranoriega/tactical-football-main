using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int movesRemaining = 0;
    [SerializeField] GridManager gridManager;
    public UnitController centerPlayerHuman;
    public UnitController centerPlayerAI;
    Labeller lastLabeller;

    
    public List<UnitController> allUnits = new List<UnitController>();

 //   private bool isFirstTurn = true;


    public List<Transform> selectedPlayers = new List<Transform>();
    public Dictionary<string, UnitController> playerUnitsByID = new Dictionary<string, UnitController>();
   

    public int maxActionsPerTurn = 12;
    private int remainingActions;

    public Dictionary<string, List<Node>> movementQueueIA = new Dictionary<string, List<Node>>();

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    
    private void Start()
    {
      
    }


     
    public void StartNewTurn()
    {
        
       
        remainingActions = maxActionsPerTurn;
        selectedPlayers.Clear();
        StartCoroutine(StartTurnDelayed());
      
    }

    private IEnumerator StartTurnDelayed()
    {
        yield return new WaitForSeconds(2f); // ⏳ espera solo en el primer turno
    //    IAInputHandler.Instance.CheckBallPossession();
       
    }


    // Llamar cada vez que spawnees un jugador
    public void RegisterUnit(UnitController unit)
    {
        if (!allUnits.Contains(unit))
            allUnits.Add(unit);
    }

    public void UnregisterUnit(UnitController unit)
    {
        if (allUnits.Contains(unit))
            allUnits.Remove(unit);
    }
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteMoves();
        }
       
    }

      public bool CanAct()
    {
        return remainingActions > 0;
    }

  

    public void ExecuteTurn()
    {
      
   
   
          movesRemaining = 0;

        foreach (var unit in allUnits)
        {
            var queue = unit.GetComponent<PlayerActionQueue>();
             var unitController = unit.GetComponent<UnitController>();
            if (queue == null) continue;

            if (queue.queuedAction.Count > 0)
            {
                movesRemaining++;
                unitController.ResetHighlight();
                // Ejecuta la cola de acciones del jugador con callback
                queue.ExecuteNextAction(OnUnitFinishedMoving);
        
            }
            // Limpia acción al final del turno
         
        }
    }



    //REVISA QUE JUGADORES FUERON SELECCIONADOS 
    //Y LUEGO LLAMA A FOLLOWPATH 
    public void ExecuteMoves()
    {
        // 1. Jugadores humanos
        ExecuteTurn();
        // ExecuteMovements();
        EndTurn();

    }

   

    private void OnUnitFinishedMoving()
    {    

        movesRemaining--;
        // if (movesRemaining <= 0)
        // {
           
        //     ExecuteShot();
        // }
       
    }
   

  
 
    public void OnGoalScored(int teamID)
    {
      
     
         movementQueueIA.Clear();
         selectedPlayers.Clear();
         StartCoroutine(ResetAfterGoal(teamID));
        // 🔑 Aquí decides quién saca: el equipo que recibió el gol
        
    }
    private void ResetUnitToInitial(UnitController unit)
    {
        var team = unit.GetComponent<PlayerTeam>();
        if (team == null) return;

        Vector3 worldPos = gridManager.GetPositionFromCoordinates(team.initialCoordinates);
        unit.transform.position = worldPos + Vector3.up * 0.5f;
    }



    private IEnumerator ResetAfterGoal(int teamID)
    {
        
        foreach (var unit in playerUnitsByID.Values)
            unit.StopMoving(unit.transform);

        foreach (var unit in IAInputHandler.Instance.aiUnitsByID.Values)
            unit.StopMoving(unit.transform);

        yield return new WaitForSeconds(1f); // ⏳ espera solo en el primer turno
        foreach (var kvp in playerUnitsByID)
        {
             ResetUnitToInitial(kvp.Value);
        }

        // Reset jugadores IA
        foreach (var kvp in IAInputHandler.Instance.aiUnitsByID)
        {
             ResetUnitToInitial(kvp.Value);
        }

        if (teamID == 1) // Gol en la portería del jugador humano
        {
            MyDebug.Log("Saca el jugador humano");
            BallManager.Instance.GiveBallTo(centerPlayerHuman.transform);
        }
        else  
        {
            // Gol en la portería de la IA
            MyDebug.Log("Saca la IA");
            BallManager.Instance.GiveBallTo(centerPlayerAI.transform);
        }

       EndTurn();
       // OnTurnStarted?.Invoke(); // avisamos al resto de sistemas
    }

      public void EndTurn()
    {
      
        movementQueueIA.Clear();
        selectedPlayers.Clear();
     
        StartNewTurn();
    }
}
