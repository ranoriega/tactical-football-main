using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{
    public Transform player;
    public abstract void Execute();
}
public enum PassType
{
    Ground,   // pase raso
    High,     // pase bombeado / alzado
    Through   // pase filtrado (opcional para después)
}
public class MoveAction : PlayerAction
{
    public List<Node> path;

    public MoveAction(Transform player, List<Node> path)
    {
        this.player = player;
        this.path = path;
    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public void ExecuteWithCallback(Action onComplete)
    {
        player.GetComponent<UnitController>()
              .StartFollowingPath(player, path, onComplete);
    }

}

public class PassAction : PlayerAction
{
    public Transform target;
    public PassType passType;

    public PassAction(Transform player, Transform target, PassType type = PassType.Ground)
    {
        this.player = player;
        this.target = target;
        this.passType = type;
    }

    public override void Execute()
    {
        // El pase lo maneja el ActionQueue
        BallManager.Instance.PassBallTo(target, passType);
    }
}

public class ShootAction : PlayerAction
{
    public Vector3 offset;

    public ShootAction(Transform player, Vector3 offset)
    {
        this.player = player;
        this.offset = offset;
    }


    public override void Execute()
    {
        // El tiro también lo maneja el ActionQueue
        player.GetComponent<PlayerActionQueue>().RegisterShot(player, offset);
        
    }
}
