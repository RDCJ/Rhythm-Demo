using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareState: GameBaseState
{
    public PrepareState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("enter PrepareState" + " current time: " + gameMgr.current_time + " Time.time: " + Time.time);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (gameMgr.current_time < 0)
        {
            gameMgr.GenerateNote();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void FrameLateUpdate()
    {
        if (gameMgr.current_time >= 0)
        {
            stateMachine.ChangeState(gameMgr.playingState);
        }
    }
}
