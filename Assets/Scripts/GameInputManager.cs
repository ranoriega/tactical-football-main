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
      
            // 🚨 Si el click está sobre UI, salimos y no procesamos raycast 
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) { return; }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ⬛ Si haces clic en un tile (suelo)
                if (hit.transform.CompareTag("Tile"))
                {
                    if (unitSelected)
                    {
                        Vector2Int targetCords = hit.transform.GetComponent<Tile>().cords;
                        Vector2Int startCords = new Vector2Int(
                            (int)selectedUnit.transform.position.x,
                            (int)selectedUnit.transform.position.z
                        ) / gridManager.UnityGridSize;
                        RecalculatePath(startCords, targetCords, selectedUnit);

                        // RecalculatePath(true);
                    }
                }

                // ⬛ Si haces clic en un jugador (Unit)
                else if (hit.transform.CompareTag("Unit"))
                {
                    Transform clickedUnit = hit.transform;
                    
                    if (isPassing)
                    {

                        // Modo pase activo
                        if (clickedUnit != passSource)
                        {
                            PlayerActionQueue passerAccion = passSource.GetComponent<PlayerActionQueue>();
                            UnitController receiverID = clickedUnit.GetComponent<UnitController>();
                             PassUI.Instance.OpenPassMenu(passSource, receiverID.transform);
                            MyDebug.Log($"{passSource.GetComponent<PlayerActionQueue>().playerID} pasara a. {receiverID.GetComponent<PlayerActionQueue>().playerID}");

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
                }

            }
        }

    }


    void TrySelectUnit(Transform unit)
    {
        selectedUnit = unit;
        unitSelected = true;
        var unitController = unit.GetComponent<UnitController>();
        if (unitController != null)
        {
            unitController.HighlightSelection(Color.green);
        }

       // path.Clear();

    }

    public void RecalculatePath(Vector2Int startCords, Vector2Int targetCords, Transform player)
    {
        // Calcula el nuevo camino desde start a target
        List<Node> path = pathFinder.GetNewPath(startCords, targetCords);

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
