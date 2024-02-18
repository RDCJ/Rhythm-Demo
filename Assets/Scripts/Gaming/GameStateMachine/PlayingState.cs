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
        Debug.Log("enter PlayingState" + " current time: " + gameMgr.current_time + " Time.time: " + Time.time);
        gameMgr.Continue();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!gameMgr.IsMusicEnd)
            gameMgr.GenerateNote();
        else
            stateMachine.ChangeState(gameMgr.musicEndState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
    }
}
