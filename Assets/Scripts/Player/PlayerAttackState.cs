using UnityEngine;

public sealed class PlayerAttackState : BaseState<PlayerController>
{
    private float attackTimer;

    public PlayerAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine)
    {
    }

    public override void Enter()
    {
        PlayAttackAnimation();
        attackTimer = owner.CurrentAttackDelay;
    }

    public override void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f || !owner.IsAttackAnimationFinished)
        {
            return;
        }

        if (owner.CheckEnemyInRange())
        {
            PlayAttackAnimation();
            attackTimer = owner.CurrentAttackDelay;
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
