using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBaseState
{
    protected StateMachine stateMachine;
    protected GameMgr gameMgr;

    public GameBaseState(GameMgr gameMgr, StateMachine stateMachine)
    {
        this.gameMgr = gameMgr;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void FrameLateUpdate() { }
}
