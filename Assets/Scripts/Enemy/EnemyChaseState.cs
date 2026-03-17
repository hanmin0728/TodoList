using UnityEngine;

public class EnemyChaseState : BaseState<EnemyBase>
{
    public EnemyChaseState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine){}

    public override void Enter()
    {
    }

    public override void Update()
    {
        //플레아어 쪽으로 다가와야함
    }

    public override void Exit()
    {
    }

    
}
