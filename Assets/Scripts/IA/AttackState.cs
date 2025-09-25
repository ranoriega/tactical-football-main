using UnityEngine;

public class AttackState : IState
{
    private IAInputHandler ai;
   
    public AttackState(IAInputHandler ai)
    {
        this.ai = ai;
     
    }

    public void Enter()
    {
        MyDebug.Log("IA entra en modo ataque");
     //   1. ¿Está en rango de tiro?
        if (ai.CanShootToGoal())
        {
            MyDebug.Log(ai.name + " decide disparar");
            ai.ShootToGoal();
            return;
        }

        // 2. ¿Hay un compañero mejor ubicado?
        var teammate = ai.GetBestPassingOption();
        if (teammate != null)
        {
            MyDebug.Log(" decide pasar a " + teammate.GetComponent<PlayerActionQueue>().playerID);
            ai.PassTo(teammate);
            return;
        }
          // 3. Si no puede ni tirar ni pasar → avanza hacia el arco rival
        MyDebug.Log(ai.name + " avanza hacia el arco");
        ai.MoveTowardsOpponentGoal(); // 👈 aquí llamas a la función que mueve a los 3 bots hacia el arco
    
    }

    public void Execute()
    {

        var currentHolder = BallManager.Instance.GetCurrentBallHolder();

        if (currentHolder != null && currentHolder.GetComponent<PlayerActionQueue>().playerID.StartsWith("player"))
        {
            // no  tiene el balón → modo defensa

            ai.StateMachine.ChangeState(new DefendState(ai));
        }

    }

    public void Exit()
    {
        MyDebug.Log(" IA sale del modo ataque");
    }
}
