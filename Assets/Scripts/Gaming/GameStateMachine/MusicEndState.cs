using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicEndState : GameBaseState
{
    public MusicEndState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("enter MusicEndState" + " Time.time: " + Time.time);
        gameMgr.audioSource.Stop();
        gameMgr.bg_videoPlayer.Stop();
        ScoreMgr.Instance.ShowFinalScore();
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
