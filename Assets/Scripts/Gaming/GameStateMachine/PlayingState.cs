using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : GameBaseState
{
    public PlayingState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("enter PlayingState");
        gameMgr.Continue();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        gameMgr.DropNote();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
    }
}
