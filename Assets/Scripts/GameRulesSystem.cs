using System.Collections.Generic;
using UnityEngine;

public class GameRulesSystem : MonoBehaviour
{
    private HashSet<string> processedConflicts = new HashSet<string>();
    string GetKey(Transform a, Transform b)
{
    return a.name.CompareTo(b.name) < 0
        ? a.name + "_" + b.name
        : b.name + "_" + a.name;
}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   public void ResolveTileConflict(Transform a, Transform b)
{
    ResolveBallDuel(a, b);
}
 public void ResolveBallDuel(Transform a, Transform b)
{
    string key = GetKey(a, b);

    if (processedConflicts.Contains(key))
        return;

    processedConflicts.Add(key);

    Transform holder = BallManager.Instance.currentHolder;

    Transform attacker = holder;
    Transform defender = holder == a ? b : a;

    float attackerScore = Random.value;
    float defenderScore = Random.value;

    Transform winner = attackerScore > defenderScore ? attacker : defender;

    BallManager.Instance.GiveBallTo(winner);

    MyDebug.Log($"Duelo: {attacker.name} vs {defender.name} → gana {winner.name}");
}
}
