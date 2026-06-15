using UnityEngine;

public class StateMachine<T>
{
    public BaseState<T> CurrentState { get; private set; }

    public void Initialize(BaseState<T> startingState)
    {
        CurrentState = startingState;
        CurrentState?.Enter(); 
    }

    public void ChangeState(BaseState<T> newState) 
    {
        if (newState == null || CurrentState == newState) 
            return;

        CurrentState?.Exit();  
        CurrentState = newState; 
        CurrentState.Enter();  
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
