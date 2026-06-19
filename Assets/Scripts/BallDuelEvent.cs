using System.Collections;
using UnityEngine;

public class BallDuelEvent : IGameEvent
{
    private Transform a;
    private Transform b;

    public BallDuelEvent(Transform a, Transform b)
    {
        this.a = a;
        this.b = b;
    }

    public IEnumerator Execute()
    {
        GameRulesSystem.Instance.ResolveTileConflict(a, b);

        yield return null;
    }
}
