using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : GameBaseState
{
    public PauseState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("enter PauseState" + " current time: " + gameMgr.current_time + " Time.time: " + Time.time);
        gameMgr.Pause();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
