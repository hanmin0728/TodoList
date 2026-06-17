using UnityEngine;

public class PlayerDieState : BaseState<PlayerController>
{
    public PlayerDieState(PlayerController context, StateMachine<PlayerController> stateMachine): base(context, stateMachine) 
    { }
    public override void Enter()
    {
        Debug.Log("다이스테이트 진입");
        owner.Anim.Play(owner.AnimDieHash, 0, 0f);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }

}
