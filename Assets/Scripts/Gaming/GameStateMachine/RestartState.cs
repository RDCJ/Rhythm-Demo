using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartState : GameBaseState
{
    public RestartState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        NotePoolManager.Instance.Reload();
        gameMgr.gestureMgr.enabled = false;
        stateMachine.ChangeState(gameMgr.initState);
    }
}
