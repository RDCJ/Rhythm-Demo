using UnityEngine;
using Note;
using GestureEvent;
using NoteGesture;

/// <summary>
/// 点击并滑动，滑动时正常判定
/// </summary>
public class Slide : NoteBase
{
    new BoxCollider2D collider2D;
    public enum SlideDirection
    {
        Left,
        Right
    }
    public SlideDirection direciton;

    protected override void Awake()
    {
        base.Awake();
        if (direciton == SlideDirection.Left)
            type = NoteType.LeftSlide;
        else
            type = NoteType.RightSlide;
        collider2D = GetComponent<BoxCollider2D>();
    }

    protected override void RegisterGestureHandler()
    {
        gestureMgr.AddListener<TapMoveRecognizer>(OnPointerSlide);
    }

    protected override void UnregisterGestureHandler()
    {
        gestureMgr.RemoveListener<TapMoveRecognizer>(OnPointerSlide);
    }

    private void OnPointerSlide(IGestureMessage msg)
    {
        if (IsActive && !IsJudged)
        {
            TapMoveMessage message = msg as TapMoveMessage;
            if (message.move_time - message.press_time < 0.2f && message.hit.collider == collider2D)
            {
                state = NoteState.Judged;
                double current_time = GameMgr.Instance.CurrentTime;
                ScoreMgr.ScoreLevel level;

                if ((message.delta_position.x < 0) == (direciton == SlideDirection.Left))
                {
                    level = ScoreMgr.Instance.JudgeClickTime(current_time, cfg.FirstCheckPoint().time);
                    //Debug.Log("sliding right " + eventData.position);
                }
                else
                {
                    level = ScoreMgr.ScoreLevel.bad;
                    //Debug.Log("sliding wrong " + eventData.position);
                }
                Debug.Log("[判定] 类型: Slide, 结果: " + level);
                ScoreMgr.Instance.AddScore(level);
                ScoreMgr.Instance.CountEarlyOrLate(current_time, cfg.FirstCheckPoint().time);
                PlayEffect(level);

                NotePoolManager.Instance.ReturnObject(this);
            }
        }
    }
}
