using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class DefendState : IState
{
    private IAInputHandler ai;
   


    public DefendState(IAInputHandler ai)
    {
        this.ai = ai;
    }

    public void Enter()
    {
        MyDebug.Log(" IA entra en modo defensa");
        ai.PlanDefense(); // usa lo que ya tiene
       
    }

    public void Execute()
    {
        // Si el AI obtiene la pelota → cambiar a ataque
         var currentHolder = BallManager.Instance.GetCurrentBallHolder();

        if (currentHolder != null && currentHolder.GetComponent<PlayerActionQueue>().playerID.StartsWith("ia"))
        {
            //  tiene el balón → modo Ataque
          
          ai.StateMachine.ChangeState(new AttackState(ai));
        }
    }
    
    public void Exit()
    {
        MyDebug.Log(" sale del modo defensa");
    }
}
