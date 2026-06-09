using UnityEngine;

public class GoalTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Busca si en el padre hay un PlayerTeam
        var team = GetComponentInParent<Goal>();
        if (team != null)
        {
            MyDebug.Log($"¡Gol al equipo {team.TeamID}!");
            TurnManager.Instance.OnGoalScored(team.TeamID);
        }
        else
        {
            MyDebug.LogWarning("No se encontró PlayerTeam en el padre de la portería.");
        }
    }
}
