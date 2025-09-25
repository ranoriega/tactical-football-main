using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> systemBehaviours;
    private List<IGameSystem> systems = new List<IGameSystem>();

    private void Start()
    {
        // Convertimos los MonoBehaviour a IGameSystem
        foreach (var behaviour in systemBehaviours)
        {
            if (behaviour is IGameSystem system)
                systems.Add(system);
            else
                MyDebug.LogWarning($"{behaviour.name} no implementa IGameSystem");
        }

        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        foreach (var system in systems)
        {
            yield return StartCoroutine(InitSystem(system));
        }

        MyDebug.Log("✅ Todos los sistemas están listos. Arranca el primer turno.");
        TurnManager.Instance.StartNewTurn();
    }

    private IEnumerator InitSystem(IGameSystem system)
    {
        bool ready = false;
        system.OnReady += () => ready = true;

        system.Initialize();
        yield return new WaitUntil(() => ready);
    }
    
  
}
