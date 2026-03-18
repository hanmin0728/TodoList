using UnityEngine;

public class EnemyAttackState : BaseState<EnemyBase>
{
    public EnemyAttackState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }

    private float attackTimer;

    // 공격 후 딜레이 상태인지 확인
    private bool isWaitingForNextAttack;

    public override void Enter()
    {
        isWaitingForNextAttack = false;
        owner.PerformAttack();
        attackTimer = owner.data.attackDelay;
    }

    public override void Update()
    {
        if (owner.IsAttackAnimationFinished && !isWaitingForNextAttack)
        {
            isWaitingForNextAttack = true; // 대기 상태 전환
            owner.Anim.Play(EnemyAnimHash.isIdle, 0, 0f);
        }

        if (owner.IsAttackAnimationFinished) //공격 대기중 플레이어 멀어질시 추적
        {
            float distance = Vector2.Distance(owner.transform.position, GameManager.Instance.Player.transform.position);
            if (distance > owner.data.attackRange)
            {
                stateMachine.ChangeState(owner.ChaseState);
                return;
            }
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f && owner.IsAttackAnimationFinished)
        {
            isWaitingForNextAttack = false;
            owner.PerformAttack();
            attackTimer = owner.data.attackDelay; 
        }
    }


    public override void Exit()
    {
     
    }
}
