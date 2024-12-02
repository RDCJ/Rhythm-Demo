using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameMgr : MonoBehaviour
{

    private void OnEnterRestartState(int lastState)
    {
        NotePoolManager.Instance.Reload();
        gestureMgr.enabled = false;
        
    }

    private void OnUpdateRestartState(float deltaTime, float elapsedTime)
    {
        FSM.TriggerAnyTransition((int)GameState.Init);
    }
}
