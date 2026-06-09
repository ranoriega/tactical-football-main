using UnityEngine;
using UnityEngine.UI;

public class ShotUI : MonoBehaviour
{
    public static ShotUI Instance;

    public GameObject panel; // Panel con 6 botones

    [SerializeField] public int target; // Panel con 6 botones

    private Transform shooter;
         [SerializeField] private Goal[] goals;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // Llamado desde tu tecla T
    public void OpenShotMenu(Transform holder)
    {
        shooter = holder;
        panel.SetActive(true);
    }

    // Llamado desde cada botón (arriba izq, centro, etc.)

    
    // --- Core: dispara a una posición local del arco ---
    private void SelectShotLocal(Vector3 localOffset, Transform goalcenter)
    {
          
        // Pasar referencia al ShotUI
        panel.SetActive(false);

        if (shooter == null)
        {
            MyDebug.LogWarning("No hay shooter al confirmar tiro.");
            return;
        }
     
        var player = shooter.GetComponent<PlayerActionQueue>();
        if (player == null)
        {
            MyDebug.LogError("PlayerActionQueue no encontrado en el shooter.");
            return;
        }
        // 
         PlayerActionQueue passerAccion = shooter.GetComponent<PlayerActionQueue>();
        if (passerAccion == null)
        {
            Debug.LogError("El tirador no tiene PlayerActionQueue");
            return;
        }
        passerAccion.QueueAction(new ShootAction(shooter, localOffset, goalcenter));
      
    
        // 

        shooter = null; // limpia referencia
    }

    // --- Wrappers para asignar en los botones (OnClick) ---
    // public void Select_TopLeft()     => SelectShotLocal(new Vector3(-1f,  1.0f, 0f));
    // public void Select_TopRight()    => SelectShotLocal(new Vector3( 1f,  1.0f, 0f));
    // public void Select_MidLeft()     => SelectShotLocal(new Vector3(-1f,  0.5f, 0f));
    // public void Select_MidRight()    => SelectShotLocal(new Vector3( 1f,  0.5f, 0f));
    public void Select_BottomRight() {
         
          int shooterID = shooter.GetComponent<PlayerTeam>().teamID;
             int targetGoalTeam = shooterID == 1 ? 2 : 1;

    Goal targetGoal = GetGoalByTeam(targetGoalTeam);
    target = targetGoal.TeamID;

    if (targetGoal == null)
    {
        Debug.LogError("No se encontró la portería rival.");
        return;
    }

    Transform goalCenter = targetGoal.goalCenter;

       
        Vector3 desiredWorldPos = new Vector3(7.13999987f, 0.77110827f, 24.1803646f);
        Vector3 offset = goalCenter.InverseTransformPoint(desiredWorldPos);
        SelectShotLocal(offset,goalCenter);
     }
    public void Select_BottomLeft()
    {
           int shooterID = shooter.GetComponent<PlayerTeam>().teamID;
              int targetGoalTeam = shooterID == 1 ? 2 : 1;

    Goal targetGoal = GetGoalByTeam(targetGoalTeam);
    target = targetGoal.TeamID;

    if (targetGoal == null)
    {
        Debug.LogError("No se encontró la portería rival.");
        return;
    }

    Transform goalCenter = targetGoal.goalCenter;

       
         Vector3 desiredWorldPos = new Vector3(3.05042219f, 0.77110827f,24.87f);
        Vector3 offset = goalCenter.InverseTransformPoint(desiredWorldPos);
        SelectShotLocal(offset,goalCenter);
       
    }
   
private Goal GetGoalByTeam(int teamID)
{
    foreach (Goal goal in goals)
    {
        if (goal.TeamID == teamID)
            return goal;
    }

    return null;
}
    // Opcional: cancelar con ESC
    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(false);
            shooter = null;
        }
    }
}
