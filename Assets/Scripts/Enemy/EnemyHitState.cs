using System.Collections;
using UnityEngine;

public class EnemyHitState : BaseState<EnemyBase>
{
    private float _hitStunDuration = 0.2f; 
    private float _timer;

    public EnemyHitState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }
    public override void Enter()
    {
        _timer = _hitStunDuration;
        owner.Anim.SetTrigger(EnemyAnimHash.isHit);

      
    }

    public override void Exit()
    {
        owner.Rigid2D.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            stateMachine.ChangeState(owner.ChaseState);
        }
    }

}
