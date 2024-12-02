using UnityEngine;

public partial class GameMgr : MonoBehaviour
{
    private void OnEnterPrepareTestState(int lastState)
    {
        PrepareTestMode();
    }

    private void OnUpdatePrepareTestState(float deltaTime, float elapsedTime)
    {
        FSM.TriggerAnyTransition((int)GameState.Playing);
    }
}
