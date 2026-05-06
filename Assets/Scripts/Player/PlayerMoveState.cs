public sealed class PlayerMoveState : BaseState<PlayerController>
{
    public PlayerMoveState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine)
    {
    }

    public override void Enter()
    {
        owner.Anim.Play(owner.AnimMoveHash, 0, 0f);
    }

    public override void Update()
    {
        if (owner.CheckEnemyInRange())
        {
            stateMachine.ChangeState(owner.AttackState);
            return;
        }

        owner.MoveRoutine();
    }

    public override void Exit()
    {
    }
}
