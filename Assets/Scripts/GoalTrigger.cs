using UnityEngine;

public class GoalTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Busca si en el padre hay un PlayerTeam
        var team = GetComponentInParent<Goal>();
        if (team != null)
        {
            MyDebug.Log($"¡Gol al equipo {team.teamID}!");
            TurnManager.Instance.OnGoalScored(team.teamID);
        }
        else
        {
            MyDebug.LogWarning("No se encontró PlayerTeam en el padre de la portería.");
        }
    }
}
