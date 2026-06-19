using System.Collections.Generic;
using UnityEngine;

public class GameRulesSystem : MonoBehaviour
{
    public static GameRulesSystem Instance;

    private void Awake()
{
    Instance = this;
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
    Transform holder = BallManager.Instance.currentHolder;

    bool aHasBall = holder == a;
    bool bHasBall = holder == b;

    // ❌ nadie tiene balón → no hay duelo
    if (!aHasBall && !bHasBall)
    {
        MyDebug.Log($"Colisión sin balón: {a.name} vs {b.name}");
        return;
    }

    // 🔥 hay balón → duelo
    ResolveBallDuel(a, b);
}
public void ResolveBallDuel(Transform a, Transform b)
{
    Transform holder = BallManager.Instance.currentHolder;

    // Seguridad por si nadie tiene el balón
    if (holder == null)
        return;

    // Seguridad por si ninguno de los dos tiene el balón
    if (holder != a && holder != b)
        return;

    Transform attacker = holder;
    Transform defender = holder == a ? b : a;

    float attackerScore =1;
    float defenderScore = 2;

    Transform winner = attackerScore > defenderScore
        ? attacker
        : defender;

    BallManager.Instance.GiveBallTo(winner);

    MyDebug.Log(
        $"Duelo: {attacker.name} vs {defender.name} → gana {winner.name}"
    );
}


}
