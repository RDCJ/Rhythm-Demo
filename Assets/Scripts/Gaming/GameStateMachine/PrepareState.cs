using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameMgr: MonoBehaviour
{
    private void OnEnterPrepareState(int lastState)
    {
        CalcPrepareTime();
        gestureMgr.enabled = true;
    }

    private void OnExitPrepareState(int nextState)
    {
        audioSource.time = (float)prepare_time;
        musicBackground.bg_videoPlayer.time = (float)prepare_time;
    }

    private void OnUpdatePrepareState(float time, float elapsedTime)
    {
        if (prepare_time < 0)
            prepare_time += time;
        if (!IsMusicEnd)
            GenerateNote();

        if (prepare_time >= 0)
        {
            FSM.TriggerAnyTransition((int)GameState.Playing);
        }
    }
}
