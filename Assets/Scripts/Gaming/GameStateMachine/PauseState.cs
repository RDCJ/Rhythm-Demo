using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : GameBaseState
{
    public PauseState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        gameMgr.Pause();
    }
}
