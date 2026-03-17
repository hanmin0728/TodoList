using UnityEngine;

public class PlayerAttackState : BaseState<PlayerController>
{
    public PlayerAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("공격 상태 진입");
        owner.Anim.SetTrigger(owner.AnimAttackHash);
    }

    public override void Update()
    {
    }


    public override void Exit()
    {
    }


}
