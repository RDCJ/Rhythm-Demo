using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public GameBaseState LastState { get; set; }
    public GameBaseState CurrentState { get; set; }

    public void Init(GameBaseState state)
    {
        CurrentState = state;
        CurrentState.EnterState();
    }

    public void ChangeState(GameBaseState state)
    {
        LastState = CurrentState;
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();
    }
}
