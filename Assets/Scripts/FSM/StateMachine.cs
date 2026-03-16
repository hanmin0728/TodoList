using UnityEngine;

public class StateMachine<T>
{
    public BaseState<T> CurrentState { get; private set; }

    // 플레이어나 적의 처음 상태 설정
    public void Initialize(BaseState<T> startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter(); // 첫 상태의 Enter를 실행
    }

    public void ChangeState(BaseState<T> newState) 
    {
        CurrentState.Exit();  
        CurrentState = newState; 
        CurrentState.Enter();  
    }
}
