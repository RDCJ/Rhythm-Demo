using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : GameBaseState
{
    public InitState(GameMgr gameMgr, StateMachine stateMachine) : base(gameMgr, stateMachine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();
        LoadingScreenManager.Instance.StartLoading(() => gameMgr.Init());
    }
}
