using UnityEngine;

public partial class GameMgr : MonoBehaviour
{
    private void OnEnterInitState(int lastState)
    {
        if (lastState == (int)GameState.Pause || lastState == (int)GameState.MusicEnd)
        {
            NotePoolManager.Instance.Reload();
            gestureMgr.enabled = false;
        }
        LoadingScreenManager.Instance.StartLoading(() => Init());
    }
}
