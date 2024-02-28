using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameBaseState
{
    protected StateMachine stateMachine;
    protected GameMgr gameMgr;
    protected string StateName => this.GetType().Name;

    public GameBaseState(GameMgr gameMgr, StateMachine stateMachine)
    {
        this.gameMgr = gameMgr;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() 
    {
        LogState();
    }

    public virtual void ExitState() 
    {
        LogState();
    }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void FrameLateUpdate() { }

    public virtual void LogState([CallerMemberName] string callerName="")
    {
        Debug.Log(string.Format("[{0}.{1}] current time: {2}, Time.time: {3}", StateName, callerName, gameMgr.CurrentTime, Time.time));
    }
}
