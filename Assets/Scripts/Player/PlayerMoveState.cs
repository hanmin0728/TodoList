using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveState : BaseState<PlayerController>
{
    public PlayerMoveState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine) { }
    public override void Enter()
    {
    }

    public override void Update()
    {
        if (owner.CheckEnemyInRange())
        {
            stateMachine.ChangeState(owner.AttackState);
        }

        GameManager.Instance.MoveBackground(owner.data.scrollSpeed);       
    }

    public override void Exit()
    {
    }
}

