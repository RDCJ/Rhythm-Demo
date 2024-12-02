using UnityEngine;

public partial class GameMgr : MonoBehaviour
{
    private void OnEnterPlayingState(int lastState)
    {
        Continue();
    }

    private void OnUpdatePlayingState(float deltaTime, float elapsedTime)
    {
        if (!IsMusicEnd)
            GenerateNote();
        else
            FSM.TriggerAnyTransition((int)GameState.MusicEnd);
    }
}
