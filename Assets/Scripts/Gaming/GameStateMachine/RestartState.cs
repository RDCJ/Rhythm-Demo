using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartState : GameBaseState
{
    public RestartState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("enter RestartState" + " current time: " + gameMgr.current_time + " Time.time: " + Time.time);
        NotePoolManager.Instance.Reload();
        stateMachine.ChangeState(gameMgr.initState);

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
