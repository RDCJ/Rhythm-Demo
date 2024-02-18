using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : GameBaseState
{
    public InitState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("enter InitState" + " current time: " + gameMgr.current_time + " Time.time: " + Time.time);
        gameMgr.Init();
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
