using UnityEngine;

public class EnemyChaseState : BaseState<EnemyBase>
{
    public EnemyChaseState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine){}

    public override void Enter()
    {
        owner.Anim.Play(EnemyAnimHash.isWalk, 0, 0f);
    }

    public override void Update()
    {
        Vector2 playerPos = GameManager.Instance.Player.transform.position;

        float distance = Vector2.Distance(owner.transform.position, playerPos);

        if (distance <= owner.data.attackRange)
        {
            owner.Rigid2D.linearVelocity = Vector2.zero;
            stateMachine.ChangeState(owner.AttackState);
            return;
        }

        owner.Rigid2D.linearVelocity = new Vector2(-owner.data.moveSpeed, owner.Rigid2D.linearVelocity.y);
    }

    public override void Exit()
    {

    }
}
