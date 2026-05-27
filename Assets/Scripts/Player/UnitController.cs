using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class UnitController : MonoBehaviour
{
    private Renderer rend;
   
  Animator animator;
     private Coroutine moveRoutine;
     private PlayerTeam team;
    [SerializeField] float movementSpeed = 1f;
    List<Node> path = new List<Node>();
    public Vector3 initialPosition;  // posición de spawn original
    GridManager gridManager;
      private void Awake()
    {
       rend = GetComponentInChildren<Renderer>();
       team = GetComponent<PlayerTeam>();

    }


    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        initialPosition = transform.position; // Guardar dónde empezó
         animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


    }
     public void HighlightSelection(Color c)
    {
     
        if (rend != null && rend.material.color != Color.cyan )
        {
            rend.material.color = c;
        }
    }

    public void ResetHighlight()
    {
        if (rend != null)
        {
            rend.material.color = team.OriginalColor;
        }
    }

    //EJECUTA EL MOVIMIENTO 
    //UNIT ES EL JUGADOR A MOVERSE
    // PATH ES EL CAMINO A RECORRER 
    public void StartFollowingPath(Transform unit, List<Node> path, System.Action onFinish)
    {
        // 🔹 Cancela cualquier tween previo del jugador
        unit.DOKill(); // Esto detiene cualquier movimiento DOTween en ese transform

        // 🔹 Llama al nuevo método DOTween para recorrer el path
        float totalTime = 0.3f * path.Count; // ajusta según velocidad deseada
        FollowPathDOTween(unit, path, totalTime, onFinish);
    }


    // public void StartFollowingPath(Transform unit, List<Node> path, System.Action onFinish)
    // {
    //     // Si ya estaba corriendo, la paramos antes de iniciar otra
    //     if (moveRoutine != null)
    //         StopCoroutine(moveRoutine);

    //     moveRoutine = StartCoroutine(FollowPath2(unit, path, onFinish));
    // }

    public void StopMoving(Transform unit)
    {
        // Detiene cualquier movimiento DOTween en este jugador
        unit.DOKill();

        // ✅ Libera la casilla actual al cancelar el movimiento
        Vector2Int currentCoords = gridManager.GetCoordinatesFromPosition(unit.position);
        Node currentNode = gridManager.GetNode(currentCoords);
        if (currentNode != null)
            currentNode.walkable = true;
    }

    // public void StopMoving(Transform unit)
    // {
    //     if (moveRoutine != null)
    //     {
    //         StopCoroutine(moveRoutine);
    //         moveRoutine = null;
    //     }

    //     // ✅ Libera la casilla actual al cancelar el movimiento
    //     Vector2Int currentCoords = gridManager.GetCoordinatesFromPosition(unit.position);
    //     Node currentNode = gridManager.GetNode(currentCoords);
    //     if (currentNode != null)
    //         currentNode.walkable = true;
    // }
    

public void FollowPathDOTween(Transform unit, List<Node> path, float totalTime, Action onComplete)
{
    if (path == null || path.Count < 2)
    {
        onComplete?.Invoke();
        return;
    }
   // 🔥 ACTIVA ANIMACIÓN AL EMPEZAR
    if (animator != null)
    {
        animator.SetFloat("isMoving", 1f);
    }
    // 🔓 Libera la casilla inicial
    Vector2Int startCords = gridManager.GetCoordinatesFromPosition(unit.position);
    Node startNode = gridManager.GetNode(startCords);
    startNode.walkable = true;

    // Construye el array de posiciones del path
    Vector3[] positions = new Vector3[path.Count];
    for (int i = 0; i < path.Count; i++)
    {
        Vector3 pos = gridManager.GetPositionFromCoordinates(path[i].cords);
        pos.y = 0.5f; // altura del jugador
        positions[i] = pos;
    }

    // DOTween: mueve al jugador a lo largo del path completo
    unit.DOPath(positions, totalTime,PathType.CatmullRom)
        .SetEase(Ease.Linear)   // movimiento uniforme, sin aceleración extra
        .SetLookAt(0.01f)       // rota el jugador suavemente hacia la dirección de movimiento
        .OnComplete(() =>
        { 
              // 🔥 DESACTIVA ANIMACIÓN AL TERMINAR
            if (animator != null)
            {
                animator.SetFloat("isMoving", 0f);
            }
            // 🔒 Bloquea la casilla final
            Vector2Int endCords = gridManager.GetCoordinatesFromPosition(unit.position);
            Node endNode = gridManager.GetNode(endCords);
            if (endNode != null)
                endNode.walkable = false;

            // ✅ Chequea robos de balón
            CheckForBallStealAfterMove(unit);

            // ✅ Notifica que terminó
            onComplete?.Invoke();
        });
}


    //REVISA POCIBLES ROBOS DE BALON
    void CheckForBallStealAfterMove(Transform movedUnit)
    {
        Transform ballHolder = BallManager.Instance.currentHolder;
        if (ballHolder == null || movedUnit == ballHolder) return;

        PlayerTeam movedTeam = movedUnit.GetComponent<PlayerTeam>();
        PlayerTeam holderTeam = ballHolder.GetComponent<PlayerTeam>();
        if (movedTeam == null || holderTeam == null) return;

        if (movedTeam.teamID != holderTeam.teamID)
        {
            Vector2Int movedCords = gridManager.GetCoordinatesFromPosition(movedUnit.position);
            Vector2Int holderCords = gridManager.GetCoordinatesFromPosition(ballHolder.position);

            if (AreAdjacent(movedCords, holderCords))
            {
                BallManager.Instance.GiveBallTo(movedUnit);
                MyDebug.Log("¡Robo automático después de moverse!");
            }
        }
        
    // 🔹 Nueva lógica: detectar frente a frente con otro jugador
    foreach (var unit in TurnManager.Instance.allUnits) // allUnits: lista de todos los jugadores en el juego
    {
        if (unit == movedUnit) continue;

        if (IsFacingEachOther(movedUnit, unit.transform))
        {
            MyDebug.Log($"{movedUnit.GetComponent<PlayerActionQueue>().playerID} está frente a frente  {unit.GetComponent<PlayerActionQueue>().playerID}");
            // Aquí solo marcas frente a frente, sin robar
            // → Más adelante podrías activar mini-combate o animación
        }
    }
    }
    // Devuelve true si 'a' y 'b' están frente a frente en la cuadrícula
public bool IsFacingEachOther(Transform a, Transform b)
{
    Vector2Int aCords = gridManager.GetCoordinatesFromPosition(a.position);
    Vector2Int bCords = gridManager.GetCoordinatesFromPosition(b.position);

    int deltaX = bCords.x - aCords.x;
    int deltaZ = bCords.y - aCords.y; // .y es Z en tu grid

    // Solo frente o atrás en fila o columna
    if ((deltaX == 0 && Mathf.Abs(deltaZ) == 1) || (deltaZ == 0 && Mathf.Abs(deltaX) == 1))
    {
        // Direcciones en 2D (XZ)
        Vector3 aDir = new Vector3(a.forward.x, 0, a.forward.z).normalized;
        Vector3 bDir = new Vector3(b.forward.x, 0, b.forward.z).normalized;

        Vector3 dirAToB = (b.position - a.position).normalized;
        Vector3 dirBToA = (a.position - b.position).normalized;

        // Comprobar que cada uno mira al otro (umbral para errores de rotación)
        float threshold = 0.7f; // ~45 grados
        if (Vector3.Dot(aDir, dirAToB) > threshold && Vector3.Dot(bDir, dirBToA) > threshold)
        {
            return true;
        }
    }

    return false;
}

  public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        int deltaX = Mathf.Abs(a.x - b.x);
        int deltaZ = Mathf.Abs(a.y - b.y); // .y es Z en tu grid
        return (deltaX + deltaZ) == 1;
    }

}