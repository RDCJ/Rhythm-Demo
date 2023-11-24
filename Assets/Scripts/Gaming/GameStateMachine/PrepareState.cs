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
        Debug.Log("enter PrepareState");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (gameMgr.current_time >= 0)
        {
            stateMachine.ChangeState(gameMgr.playingState);
        }
        else
        {
            gameMgr.DropNote();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
