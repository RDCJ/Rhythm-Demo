using UnityEngine;

public partial class GameMgr : MonoBehaviour
{
    private void OnEnterInitState(int lastState)
    {
        LoadingScreenManager.Instance.StartLoading(() => Init());
    }
}
