using UnityEngine;

public class EnemyDieState : BaseState<EnemyBase>
{
    public EnemyDieState(EnemyBase owner, StateMachine<EnemyBase> stateMachine) : base(owner, stateMachine)
    {
    }
    
    public override void Enter()
    {
        owner.Anim.SetTrigger(EnemyAnimHash.isDie);
        owner.Rigid2D.linearVelocity = Vector2.zero;
        owner.GetComponent<Collider2D>().enabled = false;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }

}
