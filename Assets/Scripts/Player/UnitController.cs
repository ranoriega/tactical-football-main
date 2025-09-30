using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Renderer rend;
   

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
        // Si ya estaba corriendo, la paramos antes de iniciar otra
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(FollowPath(unit, path, onFinish));
    }

   public void StopMoving(Transform unit)
{
    if (moveRoutine != null)
    {
        StopCoroutine(moveRoutine);
        moveRoutine = null;
    }

    // ✅ Libera la casilla actual al cancelar el movimiento
    Vector2Int currentCoords = gridManager.GetCoordinatesFromPosition(unit.position);
    Node currentNode = gridManager.GetNode(currentCoords);
    if (currentNode != null)
        currentNode.walkable = true;
}
   
public IEnumerator FollowPath(Transform unit, List<Node> patht, Action onComplete)
{
    if (patht == null || patht.Count < 2)
    {
        onComplete?.Invoke();
        yield break;
    }

    // 🔓 Libera la casilla actual
    Vector2Int startCords = gridManager.GetCoordinatesFromPosition(unit.position);
    Node startNode = gridManager.GetNode(startCords);
    startNode.walkable = true;

    for (int i = 1; i < patht.Count; i++)
    {
        Vector3 startPosition = unit.position;
        startPosition.y = 0.5f;

        Vector3 endPosition = gridManager.GetPositionFromCoordinates(patht[i].cords);
        endPosition.y = 0.5f;

        float travelPercent = 0f;

        unit.LookAt(endPosition);

        while (travelPercent < 1f)
        {
            travelPercent += Time.deltaTime * movementSpeed;
            unit.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return null;
        }
    }

    // 🔒 Bloquea la casilla final
    Vector2Int endCords = gridManager.GetCoordinatesFromPosition(unit.position);
    Node endNode = gridManager.GetNode(endCords);
    endNode.walkable = false;

    // ✅ Después de moverse, chequea si puede robar el balón
    CheckForBallStealAfterMove(unit);

    // ✅ Notifica que terminó
    onComplete?.Invoke();
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
    }
  public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        int deltaX = Mathf.Abs(a.x - b.x);
        int deltaZ = Mathf.Abs(a.y - b.y); // .y es Z en tu grid
        return (deltaX + deltaZ) == 1;
    }

}