using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;



public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance;
   

    Transform selectedUnit;
    bool unitSelected = false;
    public bool isPassing = false;
    public Transform passSource = null;

    GridManager gridManager;
    Pathfinding pathFinder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }


    void Start()
    {
      gridManager = GridManager.Instance;
        pathFinder = Pathfinding.Instance; 
    }


    // Update is called once per frame

    void Update()
    {
        
        //PRESIONA N ENTRA MODO PASE
        if (Input.GetKeyDown(KeyCode.N))
        {
              PassUI.Instance.OpenModeMenu();
            Transform unitWithBall = BallManager.Instance.GetCurrentBallHolder(); 
            if (unitWithBall != null)
            {
                isPassing = true;
                passSource = unitWithBall;
              
            }
            else
            {
                MyDebug.Log("Ningún jugador tiene el balón actualmente.");
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            var holder = BallManager.Instance.currentHolder;
            if (holder != null)
            {
                ShotUI.Instance.OpenShotMenu(holder);
            }
            else
            {
                MyDebug.LogWarning("No hay jugador con balón para abrir el menú de tiro.");
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 🧍 UNITS (físico)
                if (hit.transform.CompareTag("Unit"))
                {
                    Transform clickedUnit = hit.transform;

                    if (isPassing)
                    {
                        if (clickedUnit != passSource)
                        {
                            UnitController receiverID = clickedUnit.GetComponent<UnitController>();

                            PassUI.Instance.OpenPassMenu(passSource, receiverID.transform);

                            receiverID.HighlightSelection(Color.cyan);

                            isPassing = false;
                            passSource = null;
                        }
                        else
                        {
                            MyDebug.Log("No puedes pasarte el balón a ti mismo.");
                        }
                    }
                    else
                    {
                        TrySelectUnit(clickedUnit);
                    }

                    return; 
                }
            }

            //  TILES (GRID - SIN Physics)
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);

                Vector2Int coords = gridManager.GetCoordinatesFromPosition(worldPos);

                Node node = gridManager.GetNode(coords);

                if (node != null && unitSelected)
                {
                    Vector2Int startCords =
                        gridManager.GetCoordinatesFromPosition(selectedUnit.position);

                    RecalculatePath(startCords, coords, selectedUnit);
                }
            }
        }
     

    }


    void TrySelectUnit(Transform unit)
    {
        selectedUnit = unit;
        unitSelected = true;
        var unitController = unit.GetComponent<UnitController>();
        MovementRangeManager.Instance.ShowRange(unit);
         gridManager.RefreshTiles();
        if (unitController != null)
        {
            unitController.HighlightSelection(Color.green);
        }

       // path.Clear();

    }

    public void RecalculatePath(Vector2Int startCords, Vector2Int targetCords, Transform player)
    {
        Node targetNode = GridManager.Instance.GetNode(targetCords);

    if (!targetNode.inRange)
    {
        MyDebug.Log("Casilla fuera de rango");
        return;
    }
        // Calcula el nuevo camino desde start a target
        List<Node> path = pathFinder.GetNewPath(startCords, targetCords);
        
        if (path.Count - 1 > 5)
        {
            MyDebug.Log("Movimiento demasiado largo");
            return;
        }

        // Obtiene el componente PlayerActionQueue del jugador
        PlayerActionQueue actionQueue = player.GetComponent<PlayerActionQueue>();
       
        if (actionQueue != null)
        {
            // Guarda el camino en el jugador
          actionQueue.QueueAction(new MoveAction(player, path)); 
        }
        else
        {
            MyDebug.LogWarning($"El jugador {player.name} no tiene PlayerActionQueue.");
        }
     

    }




}
