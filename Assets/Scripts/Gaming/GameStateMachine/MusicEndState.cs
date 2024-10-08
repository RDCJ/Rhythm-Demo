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
        gameMgr.musicBackground.Stop();
        ScoreMgr.Instance.ShowFinalScore();
        gameMgr.gestureMgr.enabled = false;
    }
}
