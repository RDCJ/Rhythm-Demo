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
        gameMgr.EnterMusicEnd();
    }
}
