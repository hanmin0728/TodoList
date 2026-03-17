using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveState : BaseState<PlayerController>
{
    public PlayerMoveState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine) { }
    public override void Enter()
    {
        // 죽음이나 멈춰야하는 상태 추가될시 추가 작업
        //owner.Anim.SetBool(owner.AnimWalkHash, true);
    }

    public override void Update()
    {
        if (owner.CheckEnemyInRange())
        {
            Debug.Log("공격상태 진입");
            stateMachine.ChangeState(owner.AttackState);
        }

        GameManager.Instance.MoveBackground(owner.data.scrollSpeed);       
    }

    public override void Exit()
    {
        // 죽음이나 멈춰야하는 상태 추가될시 추가 작업
        //owner.Anim.SetBool(owner.AnimWalkHash, false);
    }
}

