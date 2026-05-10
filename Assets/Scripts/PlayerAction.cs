using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase base de todas las acciones del jugador (mover, pasar, tirar).
/// Define una ejecución básica y otra con callback.
/// </summary>
public abstract class PlayerAction
{
    public Transform player;
 public Transform goalcenter;
    /// <summary>
    /// Ejecuta la acción (sin esperar resultado).
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Ejecuta la acción y avisa cuando termina.
    /// IMPORTANTE: Esto permite encadenar acciones en orden.
    /// </summary>
    public virtual void ExecuteWithCallback(Action onComplete)
    {
        Execute(); // Ejecuta normalmente
        onComplete?.Invoke(); // Termina inmediatamente (por defecto)
    }
}
public enum PassType
{
    Ground,   // pase raso
    High,     // pase bombeado / alzado
    Through,  // pase filtrado (opcional para después)
    curve,
    combine
}
/// <summary>
/// Acción de movimiento siguiendo un path.
/// </summary>
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
        // No se usa directamente
        throw new NotImplementedException();
    }

    /// <summary>
    /// Mueve al jugador y espera hasta terminar el movimiento.
    /// </summary>
    public override void ExecuteWithCallback(Action onComplete)
    {
        player.GetComponent<UnitController>()
              .StartFollowingPath(player, path, onComplete);
    }
}

/// <summary>
/// Acción de pase de balón.
/// </summary>
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
        // No se usa directamente
    }

    /// <summary>
    /// Ejecuta el pase y espera a que termine la animación del balón.
    /// </summary>
    public override void ExecuteWithCallback(Action onComplete)
    {
        BallManager.Instance.PassBallTo(target, passType, onComplete);
    }
}

/// <summary>
/// Acción de tiro a portería.
/// </summary>
public class ShootAction : PlayerAction
{
    public Vector3 offset;

    public ShootAction(Transform player, Vector3 offset, Transform goalcenter)
    {
        this.player = player;
        this.offset = offset;
        this.goalcenter = goalcenter;
    }

    public override void Execute()
    {
        // No se usa directamente
    }

    /// <summary>
    /// Registra el tiro y espera un pequeño tiempo (simulación de animación).
    /// </summary>
    /// 
   

    public override void ExecuteWithCallback(Action onComplete)
    {
    
       BallManager.Instance.ShotBallTo(player, goalcenter,offset,onComplete);
    }

   
}
