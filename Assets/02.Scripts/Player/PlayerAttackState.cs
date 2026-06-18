using UnityEngine;

public sealed class PlayerAttackState : BaseState<PlayerController>
{
    private float attackTimer;
    private bool isWaitingForNextAttack; 

    public PlayerAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine)
    {
    }

    public override void Enter()
    {
        isWaitingForNextAttack = false;
        attackTimer = owner.CurrentAttackDelay;
        PlayAttackAnimation();
    }

    public override void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        if (!owner.IsAttackAnimationFinished)
        {
            return;
        }

        if (attackTimer > 0f && !isWaitingForNextAttack)
        {
            isWaitingForNextAttack = true;
            owner.Anim.Play(owner.AnimIdleHash, 0, 0f);
            return;
        }

        if (attackTimer > 0f) return;

        if (owner.IsAttackAnimationFinished)
        {
            if (!owner.CheckEnemyInRange())
            {
                stateMachine.ChangeState(owner.MoveState);
                return;
            }

            //아직 살아있는 적이 있다면 공격 재시작
            if (attackTimer <= 0f)
            {
                attackTimer = owner.CurrentAttackDelay;

                PlayAttackAnimation();
            }
        }
    }

    public override void Exit()
    {
    }

    private void PlayAttackAnimation()
    {
        owner.IsAttackAnimationFinished = false;
        owner.Anim.Play(owner.AnimAttackHash, 0, 0f);
    }
}
