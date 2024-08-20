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
        gameMgr.CalcPrepareTime();
        gameMgr.gestureMgr.enabled = true;
    }

    public override void ExitState()
    {
        base.ExitState();
        gameMgr.audioSource.time = (float)gameMgr.prepare_time;
        gameMgr.musicBackground.bg_videoPlayer.time = (float)gameMgr.prepare_time;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (gameMgr.prepare_time < 0)
            gameMgr.prepare_time += Time.deltaTime;
        if (!gameMgr.IsMusicEnd)
            gameMgr.GenerateNote();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void FrameLateUpdate()
    {
        if (gameMgr.prepare_time >= 0)
        {
            stateMachine.ChangeState(gameMgr.playingState);
        }
    }
}
