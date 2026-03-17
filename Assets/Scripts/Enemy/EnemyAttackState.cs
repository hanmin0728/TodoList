using UnityEngine;

public class EnemyAttackState : BaseState<EnemyBase>
{
    public EnemyAttackState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }

    public override void Enter()
    {
        owner.PerformAttack(); 
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }


}
