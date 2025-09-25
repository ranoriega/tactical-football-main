using UnityEngine;

public class OpponentSpawner : MonoBehaviour
{
    [SerializeField] GameObject opponentPrefab; // Prefab equipo 2
    [SerializeField] GridManager gridManager;
    [SerializeField] Vector2Int[] spawnPositions;
    [SerializeField] Color opponentColor = Color.blue; // Puedes cambiarlo desde el Inspector


    void Start()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Vector2Int pos = spawnPositions[i];
            Vector3 spawnPos = gridManager.GetPositionFromCoordinates(pos);
            spawnPos += Vector3.up * 0.5f;  // Ajusta 0.4f según el tamaño de tu jugador
            GameObject opponent = Instantiate(opponentPrefab, spawnPos, Quaternion.identity);
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
            }
        }
      
    }
}
