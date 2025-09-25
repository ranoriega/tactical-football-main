using UnityEngine;
using System.Collections.Generic;

public class PlayerSelector : MonoBehaviour
{
    // Lista de jugadores seleccionados
    private List<GameObject> selectedPlayers = new List<GameObject>();

    // Color de resaltado
    public Color highlightColor = Color.yellow;

    // Para guardar colores originales de cada jugador
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

    public void SelectPlayer(GameObject player)
    {
        if (!selectedPlayers.Contains(player))
        {
            var renderer = player.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Guardamos el color original si no estaba guardado
                if (!originalColors.ContainsKey(player))
                    originalColors[player] = renderer.material.color;

                renderer.material.color = highlightColor;
            }

            selectedPlayers.Add(player);
        }
    }

    public void DeselectPlayer(GameObject player)
    {
        if (selectedPlayers.Contains(player))
        {
            var renderer = player.GetComponent<Renderer>();
            if (renderer != null && originalColors.ContainsKey(player))
                renderer.material.color = originalColors[player]; // Restaurar color

            selectedPlayers.Remove(player);
        }
    }

    // Opción rápida para limpiar todos de golpe
    public void DeselectAll()
    {
        foreach (var player in selectedPlayers)
        {
            var renderer = player.GetComponent<Renderer>();
            if (renderer != null && originalColors.ContainsKey(player))
                renderer.material.color = originalColors[player];
        }

        selectedPlayers.Clear();
    }
}
