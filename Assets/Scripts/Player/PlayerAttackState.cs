using UnityEngine;

public class PlayerAttackState : BaseState<PlayerController>
{
    public PlayerAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine) { }

    private float _attackTimer;
    public override void Enter()
    {
        owner.Anim.SetTrigger(owner.AnimAttackHash);
        _attackTimer = owner.data.attackDelay;
    }

    public override void Update()
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0)
        {
            // 앞에 적이 여전히 있다면 다시 공격
            if (owner.CheckEnemyInRange())
            {
                owner.Anim.SetTrigger(owner.AnimAttackHash);
                _attackTimer = owner.data.attackDelay;
            }
            // 적이 없다면 다시 걷기 상태로
            else
            {
                stateMachine.ChangeState(owner.MoveState);
            }
        }
    }


    public override void Exit()
    {
    }


}
