using System.Collections;
using UnityEngine;

public class EnemyHitState : BaseState<EnemyBase>
{
    private readonly float hitStunDuration = 0.3f;
    private float timer;

    public EnemyHitState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }
    public override void Enter()
    {
        timer = hitStunDuration;
        owner.Anim.Play(EnemyAnimHash.isHit, 0, 0f);
    }

    public override void Exit()
    {
        owner.Rigid2D.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            stateMachine.ChangeState(owner.ChaseState);
        }
    }

}
