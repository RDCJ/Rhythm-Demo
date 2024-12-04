using UnityEngine;
using Note;
using NoteGesture;
using GestureEvent;

/// <summary>
/// 滑动，判定框小，必判perfect
/// </summary>
public class Catch : NoteBase
{
    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Catch;
    }

    private void OnPointerAction(IGestureMessage msg)
    {
        if (this.IsActive && !this.IsJudged)
        {
            var message = msg as SimpleGestureMessage;
            float gestureLocalPositionX = message.position.x - Screen.width / 2;
            if (gestureLocalPositionX > TouchAreaLeftBoundX && gestureLocalPositionX < TouchAreaRightBoundX)
            {
                state = NoteState.Judged;

                ScoreMgr.ScoreLevel level = ScoreMgr.ScoreLevel.perfect;
                // 计分
                Debug.Log("[判定] 时间: " + GameMgr.Instance.CurrentTime.ToString("N4") + "类型: Catch, 结果: " + level);
                GameMgr.Instance.AddScore(level);
                // 点击效果
                PlayEffect(level);
                //
                NotePoolManager.Instance.ReturnObject(this);
            }
        }
    }

    protected override float TouchAreaLength
    {
        get
        {
            return PlayerPersonalSetting.ScaledDropSpeed * JudgeIntervalConfig.active_interval * 2;
        }
    }

    protected override void RegisterGestureHandler()
    {
        gestureMgr.AddListener<BeganGestureRecognizer>(OnPointerAction);
        gestureMgr.AddListener<StationaryGestureRecognizer>(OnPointerAction);
        gestureMgr.AddListener<MoveGestureRecognizer>(OnPointerAction);
    }

    protected override void UnregisterGestureHandler()
    {
        gestureMgr.RemoveListener<BeganGestureRecognizer>(OnPointerAction);
        gestureMgr.RemoveListener<StationaryGestureRecognizer>(OnPointerAction);
        gestureMgr.RemoveListener<MoveGestureRecognizer>(OnPointerAction);
    }
}
