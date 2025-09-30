using System;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour, IGameSystem
{
    public static PlayerSpawner Instance;
    private Renderer rend;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject opponentPrefab; // Prefab equipo 2
    [SerializeField] GridManager gridManager;
    [SerializeField] Vector2Int[] spawnPositions;
    [SerializeField] Vector2Int[] spawnPositions2;
    [SerializeField] Color opponentColor = Color.blue; // Puedes cambiarlo desde el Inspector
    [SerializeField] Color playerColor = Color.red; // Puedes cambiarlo desde el Inspector
    
    public Transform lastSpawnedPlayer; // ← Esto será el jugador del centro
    // Declaramos un evento sin parámetros
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

    public void SpawnPlayers()
    {
        // Código que spawnea jugadores...
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Vector2Int pos = spawnPositions[i];
            Vector3 worldPos = gridManager.GetPositionFromCoordinates(pos);
            // Ajustamos la altura para que el jugador no se hunda
            worldPos += Vector3.up * 0.5f;  // Ajusta 0.4f según el tamaño de tu jugador
            GameObject player = Instantiate(playerPrefab, worldPos, Quaternion.identity);
            lastSpawnedPlayer = player.transform;
            if (i == spawnPositions.Length / 2) // o directamente la posición de kickoff
            {
                TurnManager.Instance.centerPlayerHuman = player.GetComponent<UnitController>();
            }
            
            PlayerTeam team = player.GetComponent<PlayerTeam>();
            if (team != null)
            {
                team.teamID = 1; // ← Este es el equipo local
                team.initialCoordinates = pos;
                rend = player.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    team.OriginalColor = rend.material.color; // guardamos el color inicial
                }
        
            }
            // Asignar ID único al jugador
            UnitController unitCtrl = player.GetComponent<UnitController>();
            PlayerActionQueue IdPlayer = player.GetComponent<PlayerActionQueue>();
            if (unitCtrl != null)
            {
                IdPlayer.playerID = "player" + i;

                if (TurnManager.Instance != null)
                {
                    TurnManager.Instance.playerUnitsByID[IdPlayer.playerID] = unitCtrl;
                    TurnManager.Instance.RegisterUnit(unitCtrl);
                }
                else
                {
                    MyDebug.LogWarning("TurnManager.Instance no está disponible al registrar jugador");
                }
            }
            // Si es IA, guarda referencia

        }

        BallManager.Instance.GiveBallTo(lastSpawnedPlayer);

        // Notificamos a todos los suscriptores que los jugadores ya fueron spawneados

    }

    public void SpawnAIPlayers()
    {
        for (int i = 0; i < spawnPositions2.Length; i++)
        {
            Vector2Int pos = spawnPositions2[i];
            Vector3 spawnPos = gridManager.GetPositionFromCoordinates(pos);
            spawnPos += Vector3.up * 0.5f;  // Ajusta 0.4f según el tamaño de tu jugador
            GameObject opponent = Instantiate(opponentPrefab, spawnPos, Quaternion.identity);
             if (i == spawnPositions.Length / 2)
            {
                TurnManager.Instance.centerPlayerAI = opponent.GetComponent<UnitController>();
            }
            // 🔵 Cambiar color visual
            Renderer rend = opponent.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material.color = opponentColor;
            }

            UnitController unitCtrl = opponent.GetComponent<UnitController>();
            PlayerActionQueue IdPlayer = opponent.GetComponent<PlayerActionQueue>();

            if (unitCtrl != null)
            {
                IdPlayer.playerID = "iaPlayer" + i;

                if (IAInputHandler.Instance != null)
                {
                    // ✅ REGISTRAR EN EL DICCIONARIO DE LA IA
                    IAInputHandler.Instance.aiUnitsByID[IdPlayer.playerID] = unitCtrl;
                    TurnManager.Instance.RegisterUnit(unitCtrl);

                }
                else
                {
                    MyDebug.LogWarning("IAInputHandler.Instance no está disponible al registrar IA");
                }
            }
            // Aquí asignas el equipo
            PlayerTeam team = opponent.GetComponent<PlayerTeam>();
            if (team != null)
            {
                team.teamID = 2; // ← Este es el equipo del oponente
                team.initialCoordinates = pos;
                team.OriginalColor = opponentColor;
            }
         
        }
     
      }
    // Pathfinding pathFinder;



    public void Initialize()
    {
      
        SpawnAIPlayers();
        SpawnPlayers();
        OnReady?.Invoke();
    }
}
