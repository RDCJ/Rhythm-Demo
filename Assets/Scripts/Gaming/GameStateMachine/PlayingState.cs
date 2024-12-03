using UnityEngine;

public partial class GameMgr : MonoBehaviour
{
    private void OnEnterPlayingState(int lastState)
    {
        if (lastState == (int)GameState.Init)
        {
            if (is_test_mode)
            {
                PrepareTestMode();
                Continue();
            }
            else
            {
                CalcPrepareTime();
                gestureMgr.enabled = true;
                if (prepare_time >= 0)
                {
                    audioSource.time = (float)prepare_time;
                    musicBackground.bg_videoPlayer.time = (float)prepare_time;
                    Continue();
                }
            }
            
        }
        else
            Continue();
    }

    private void OnUpdatePlayingState(float deltaTime, float elapsedTime)
    {
        if (prepare_time < 0)
        {
            prepare_time += deltaTime;
            if (prepare_time >= 0)
            {
                audioSource.time = (float)prepare_time;
                musicBackground.bg_videoPlayer.time = (float)prepare_time;
                Continue();
            }
        }
            
        if (!IsMusicEnd)
            GenerateNote();
        else
            FSM.TriggerTransition((int)GameStateTransitionEvent.GameEnd);
    }
}
