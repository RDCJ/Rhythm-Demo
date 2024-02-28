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
        gameMgr.Continue();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!gameMgr.IsMusicEnd)
            gameMgr.GenerateNote();
        else
            stateMachine.ChangeState(gameMgr.musicEndState);
    }
}
