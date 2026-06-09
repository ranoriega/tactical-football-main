using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupManager : MonoBehaviour,IGameSystem
{
    private Renderer rend;
    [SerializeField] private List<UnitController> playerTeam;
    [SerializeField] private List<UnitController> aiTeam;   
    [SerializeField] GridManager gridManager;
    [SerializeField] Vector2Int[] spawnPositions;
    [SerializeField] Vector2Int[] spawnPositions2;
    [SerializeField] Color opponentColor = Color.blue; // Puedes cambiarlo desde el Inspector
    private Transform lastSpawnedPlayer; // ← Esto será el jugador del centro

    public event Action OnReady;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        private void PlacePlayers()
    {
        for (int i = 0; i < playerTeam.Count; i++)
        {
            
            Vector2Int pos = spawnPositions[i];
            Vector3 worldPos = gridManager.GetPositionFromCoordinates(pos);
            playerTeam[i].transform.position = worldPos + Vector3.up * 0.5f;
            var player =  playerTeam[i];
              if (i == spawnPositions.Length / 2) // o directamente la posición de kickoff
            {
                TurnManager.Instance.centerPlayerHuman = player.GetComponent<UnitController>();
            }
             lastSpawnedPlayer = player.transform;
            PlayerTeam team = player.GetComponent<PlayerTeam>();
            if (team != null)
                {
                    team.teamID = 1; // ← Este es el equipo local
                    team.initialCoordinates = pos;
                    rend = player.GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        team.OriginalColor = rend.materials[1].color; // guardamos el color inicial
                    }
            
                }
        }

        for (int i = 0; i < aiTeam.Count; i++)
        {
             Vector2Int pos = spawnPositions2[i];
            Vector3 worldPos = gridManager.GetPositionFromCoordinates(pos);
            aiTeam[i].transform.position = worldPos + Vector3.up * 0.5f;
            var player =  aiTeam[i];
             if (i == spawnPositions2.Length / 2)
            {
                TurnManager.Instance.centerPlayerAI = player.GetComponent<UnitController>();
            }
            PlayerTeam team = player.GetComponent<PlayerTeam>();
            if (team != null)
                {
                    team.teamID = 2; // ← Este es el equipo local
                    team.initialCoordinates = pos;
                    rend = player.GetComponentInChildren<Renderer>();
                     if (rend != null)
                    {
                    rend.materials[1].color =opponentColor;
                    }
            
                }
        }
    }


        private void RegisterPlayers()
    {
        for (int i = 0; i < playerTeam.Count; i++)
        {
            
            var unit = playerTeam[i];
            unit.GetComponent<PlayerActionQueue>().playerID = "player" + i;

            TurnManager.Instance.playerUnitsByID[unit.GetComponent<PlayerActionQueue>().playerID] = unit;
            TurnManager.Instance.RegisterUnit(unit);
        }

        for (int i = 0; i < aiTeam.Count; i++)
        {
            var unit = aiTeam[i];

             unit.GetComponent<PlayerActionQueue>().playerID = "ai" + i;

            IAInputHandler.Instance.aiUnitsByID[unit.GetComponent<PlayerActionQueue>().playerID] = unit;
            TurnManager.Instance.RegisterUnit(unit);
        }
    }
    private void SetupBall()
    {
    
        BallManager.Instance.GiveBallTo(lastSpawnedPlayer);
    }

    public void Initialize()
    {
    RegisterPlayers();
    PlacePlayers();
    SetupBall();

    OnReady?.Invoke();
    }
}
