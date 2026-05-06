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

        if (owner.CheckEnemyInRange())
        {
            isWaitingForNextAttack = false;
            attackTimer = owner.CurrentAttackDelay;
            PlayAttackAnimation();
            return;
        }

        stateMachine.ChangeState(owner.MoveState);
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
