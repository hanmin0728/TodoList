using UnityEngine;

public sealed class EnemyChaseState : BaseState<EnemyBase>
{
    public EnemyChaseState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }

    public override void Enter()
    {
        owner.Anim.Play(EnemyAnimHash.isWalk, 0, 0f);
    }

    public override void Update()
    {
        if (owner.TargetPlayer == null)
        {
            owner.Rigid2D.linearVelocity = Vector2.zero;
            return;
        }

        if (owner.IsTargetInAttackRange())
        {
            owner.Rigid2D.linearVelocity = Vector2.zero;
            stateMachine.ChangeState(owner.AttackState);
            return;
        }

        owner.Rigid2D.linearVelocity = new Vector2(-owner.Data.MoveSpeed, owner.Rigid2D.linearVelocity.y);
    }

    public override void Exit()
    {
    }
}


