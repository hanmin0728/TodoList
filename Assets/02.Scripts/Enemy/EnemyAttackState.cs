using UnityEngine;

public sealed class EnemyAttackState : BaseState<EnemyBase>
{
    private float attackCooldown;

    public EnemyAttackState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }

    public override void Enter()
    {
        owner.Rigid2D.linearVelocity = Vector2.zero; 
        ExecuteAttack();
    }

    public override void Update()
    {
        if (!owner.IsAttackAnimationFinished) return;

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
            return; 
        }

        // 3. 쿨타임이 끝났다면, 플레이어가 도망갔는지(넉백됐는지) 확인
        if (!owner.IsTargetInAttackRange())
        {
            stateMachine.ChangeState(owner.ChaseState);
            return;
        }

        ExecuteAttack();
    }

    public override void Exit()
    {
    }

    private void ExecuteAttack()
    {
        owner.PerformAttack();
        attackCooldown = owner.Data.AttackDelay;
    }
}




