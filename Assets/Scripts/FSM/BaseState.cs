using UnityEngine;

public abstract class BaseState<T>
{
    protected T owner;
    protected StateMachine<T> stateMachine;


    public BaseState(T owner, StateMachine<T> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;

    }
    public abstract void Enter(); // 상태 진입 시 1회 호출

    public abstract void Update(); // 매 프레임 호출

    public abstract void Exit(); // 상태 전환 시 1회 호출
}
