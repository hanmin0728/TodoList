using UnityEngine;

public class EnemyAttackState : BaseState<EnemyBase>
{
    public EnemyAttackState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }

    private float _attackTimer;

    public override void Enter()
    {
        owner.PerformAttack();
        _attackTimer = owner.data.attackDelay;

        owner.Anim.SetBool(EnemyAnimHash.isIdle, true);
    }

    public override void Update()
    {
        //공격 대기중에 거리가 멀어질시 추적 상태로 전환
        float distance = Vector2.Distance(owner.transform.position, GameManager.Instance.Player.transform.position);
        if (distance > owner.data.attackRange)
        {
            stateMachine.ChangeState(owner.ChaseState);
            return; 
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f) //딜레이 기다리고 공격
        {
            owner.PerformAttack(); 
            _attackTimer = owner.data.attackDelay; 
        }
    }


    public override void Exit()
    {
        owner.Anim.SetBool(EnemyAnimHash.isIdle, false);
    }
}
