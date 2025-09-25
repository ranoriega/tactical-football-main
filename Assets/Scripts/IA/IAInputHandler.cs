using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class IAInputHandler : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public static IAInputHandler Instance;
    Transform targetOpponent;
    List<Node> patht = new List<Node>();
    public Dictionary<string, UnitController> aiUnitsByID = new Dictionary<string, UnitController>();
    UnitController ballHolder;
    
    public float shootRange = 8f; // puedes ajustarlo en el inspector

    GridManager gridManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created  GridManager gridManager;
    Pathfinding pathFinder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    void Start()
    {
        if (StateMachine == null)
            StateMachine = new StateMachine();
        gridManager = FindAnyObjectByType<GridManager>();
        pathFinder = FindAnyObjectByType<Pathfinding>();

        // 🔑 Suscribirse aquí, ya que para este punto TurnManager ya existe

    }



    public bool CanShootToGoal()
{
        Transform player = BallManager.Instance.GetCurrentBallHolder();
        PlayerActionQueue teamID = player.GetComponent<PlayerActionQueue>();
        int team = teamID.GetComponent<PlayerTeam>().teamID;
        targetOpponent = teamID.GetOpponentGoal(team);
        // Ejemplo simplificado: está en cierto rango de distancia al arco
        float dist = Vector3.Distance(player.position, targetOpponent.position);
        return dist < shootRange; // shootRange = distancia máxima para disparar
}

public UnitController GetBestPassingOption()
{
    // Quién tiene la pelota
    Transform holderT = BallManager.Instance.GetCurrentBallHolder();
    if (holderT == null) return null;

    var passer = holderT.GetComponent<UnitController>();
    var passerTeam = holderT.GetComponent<PlayerTeam>()?.teamID ?? -1;
    if (passer == null || passerTeam == -1) return null;

    // Candidatos: IA del mismo equipo, distintos del pasador
    var candidates = aiUnitsByID
        .Where(kvp => kvp.Key.StartsWith("ia"))
        .Select(kvp => kvp.Value)
        .Where(u => u != null
                 && u != passer
                 && u.GetComponent<PlayerTeam>()?.teamID == passerTeam)
        .ToList();

    UnitController best = null;
    float bestScore = float.NegativeInfinity;

    foreach (var mate in candidates)
    {
        float score = EvaluatePassingOption(passer, mate);
        if (score > bestScore)
        {
            bestScore = score;
            best = mate;
        }
    }

    // Si ninguna opción es buena, devuelve null
    // (por si quieres que, en ese caso, avance con el balón)
    return best;
}


    private float EvaluatePassingOption(UnitController passer,UnitController teammate)
    {
        PlayerActionQueue teamID = passer.GetComponent<PlayerActionQueue>();
        int team = teamID.GetComponent<PlayerTeam>().teamID;
        targetOpponent = teamID.GetOpponentGoal(team);
        // Criterios: más cerca del arco y sin enemigos bloqueando la línea de pase
        float distToGoal = Vector3.Distance(teammate.transform.position, targetOpponent.position);
        float distToBall = Vector3.Distance(teammate.transform.position, transform.position);

        if (distToGoal < Vector3.Distance(transform.position, targetOpponent.position))
            return 100 - distToBall; // favorece compañeros más adelantados y cercanos

        return -999; // descartado
    }

    public void ShootToGoal()
    {
        if (BallManager.Instance.GetCurrentBallHolder() != null)
        {
            Transform player = BallManager.Instance.GetCurrentBallHolder();
            PlayerActionQueue action = player.GetComponent<PlayerActionQueue>();
            action.RegisterShot(player,new Vector3(-1f,  1.0f, 0f));
        }
        else
        {
            MyDebug.LogWarning("No hay jugador con la pelota para registrar tiro.");
        }
    }

    public void PassTo(UnitController teammate)
    {
        var player = BallManager.Instance.GetCurrentBallHolder()?.GetComponent<PlayerActionQueue>();
        if (player != null)
        {
            // En lugar de pasar directamente → se guarda en la cola
            player.QueuePass(teammate.transform);
        }
    }


public void MoveTowardsOpponentGoal()
{
    List<UnitController> selectedAI = GetSelected();
    if (selectedAI.Count == 0) return;

      Transform player = BallManager.Instance.GetCurrentBallHolder();
        PlayerActionQueue teamID = player.GetComponent<PlayerActionQueue>();
        int team = teamID.GetComponent<PlayerTeam>().teamID;
        targetOpponent = teamID.GetOpponentGoal(team);
        Vector2Int goalCoords = gridManager.GetCoordinatesFromPosition(targetOpponent.position);

    foreach (var ai in selectedAI)
    {
        if (ai == null) continue;

        Vector2Int aiCoords = gridManager.GetCoordinatesFromPosition(ai.transform.position);

        // Cada bot recalcula camino directo al arco rival 
        MyDebug.Log($"{ai.name} avanza hacia el arco");
        Recalculate(aiCoords, goalCoords, ai.transform);
    }
}


    void Update()
    {
      
        StateMachine.Update();
    }
    public void PlanDefense()
    {

        // Selección de bots, marcar jugadores, recalcular rutas, etc.
     List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        ballHolder = BallManager.Instance.GetCurrentBallHolder().GetComponent<UnitController>();
        List<UnitController> selectedAI = GetSelected();
     

        Vector2Int ballHolderCoords = gridManager.GetCoordinatesFromPosition(ballHolder.transform.position);

        Vector2Int targetForBot = ballHolderCoords; // default
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = ballHolderCoords + dir;
            if (gridManager.Grid.ContainsKey(neighbor) && gridManager.Grid[neighbor].walkable)
            {
                targetForBot = neighbor;
                break;
            }

        }

        // 4. El primero presiona al que tiene el balón
        if (selectedAI.Count > 0 && selectedAI[0] != null)
        {
            UnitController chaser = selectedAI[0];

            Vector2Int startCords = new Vector2Int(
                Mathf.RoundToInt(chaser.transform.position.x),
                Mathf.RoundToInt(chaser.transform.position.z)
            ) / gridManager.UnityGridSize;

             Recalculate(startCords, targetForBot, chaser.transform);
        }


        // 5. Obtener los jugadores sin balón para marcar
        List<UnitController> playersToMark = TurnManager.Instance.playerUnitsByID.Values
        .Where(p => p != ballHolder)
        .OrderBy(p => (p.transform.position - ballHolder.transform.position).sqrMagnitude) // los más cercanos al balón
        .ToList();


        // 6. Hacer que los otros bots marquen a esos jugadores
        for (int i = 1; i < selectedAI.Count && i - 1 < playersToMark.Count; i++)
        {
            UnitController ai = selectedAI[i];
            UnitController target = playersToMark[i - 1];

            Vector2Int aiCoords = gridManager.GetCoordinatesFromPosition(ai.transform.position);
            Vector2Int targetCoords = gridManager.GetCoordinatesFromPosition(target.transform.position);

            // Buscar casillas vecinas al jugador objetivo para marcar (pero no en su misma casilla)
            Vector2Int markPosition = targetCoords;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = targetCoords + dir;
                if (gridManager.Grid.ContainsKey(neighbor) && gridManager.Grid[neighbor].walkable)
                {
                    markPosition = neighbor;
                    break;
                }
            }

            // Recalcular camino del bot hacia esa casilla de marcaje
             Recalculate(aiCoords, markPosition, ai.transform);
        }
}
  

    public void Recalculate(Vector2Int startCords, Vector2Int targetCords, Transform playerIA)
    {

        patht.Clear();
        StopAllCoroutines();
        // Calcula el nuevo camino desde start a target
        List<Node> path = pathFinder.GetNewPath(startCords, targetCords);

        // Obtiene el componente PlayerActionQueue del jugador
        PlayerActionQueue actionQueue = playerIA.GetComponent<PlayerActionQueue>();
        if (actionQueue != null)
        {
          //  MyDebug.Log($"Path calculado para {actionQueue.playerID}: {path.Count} nodos");

            // Guarda el camino en el jugador
            actionQueue.SetMoveTarget(path);
            TurnManager.Instance.selectedPlayers.Add(playerIA);
        }
        else
        {
            MyDebug.LogWarning($"El jugador {playerIA.name} no tiene PlayerActionQueue.");
        }
    }

    /**obtener a los tres jugadores mas cercanos**/
    public List<UnitController> GetSelected()
    {

        List<UnitController> iaUnits = aiUnitsByID
        .Where(kvp => kvp.Key.StartsWith("ia"))
        .Select(kvp => kvp.Value)
        .ToList();

        // 1. Obtener al jugador con balón desde BallManager
        ballHolder = BallManager.Instance.GetCurrentBallHolder().GetComponent<UnitController>();
        if (ballHolder == null) MyDebug.Log("balon vacio");

        // 2. Ordenar IA por distancia al que tiene el balón
        Dictionary<UnitController, float> iaByDistance = new();
        foreach (var ai in iaUnits)
        {
            if (ai == null) continue;
            float distance = (ai.transform.position - ballHolder.transform.position).sqrMagnitude;
            iaByDistance[ai] = distance;
        }

        // 3. Tomar los 3 bots más cercanos
        List<UnitController> selectedAI = iaByDistance
            .OrderBy(pair => pair.Value)
            .Select(pair => pair.Key)
            .Take(3)
            .ToList();

        return selectedAI;
    }
 
 public void CheckBallPossession()
{
    var currentHolder = BallManager.Instance.GetCurrentBallHolder();

        if (currentHolder != null && currentHolder.GetComponent<PlayerActionQueue>().playerID.StartsWith("player"))
        {
            // ❌ No tiene el balón → modo defensa
            StateMachine.ChangeState(new DefendState(this));
        }
        else
        {
          StateMachine.ChangeState(new AttackState(this));
        }
   
}
        

}
