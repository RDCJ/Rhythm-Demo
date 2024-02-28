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
        gameMgr.audioSource.Stop();
        gameMgr.bg_videoPlayer.Stop();
        ScoreMgr.Instance.ShowFinalScore();
    }
}
