using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using GestureEvent;
using NoteGesture;

/// <summary>
/// 点击，正常判定
/// </summary>
public class Tap : NoteBase
{
    new BoxCollider2D collider2D;
    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Tap;
        collider2D = GetComponent<BoxCollider2D>();
    }

    protected override void RegisterGestureHandler()
    {
        gestureMgr.AddListener<BeganGestureRecognizer>(OnPointerDown);
    }

    protected override void UnregisterGestureHandler()
    {
        gestureMgr.RemoveListener<BeganGestureRecognizer>(OnPointerDown);
    }

    public void OnPointerDown(IGestureMessage msg)
    {
        if (this.IsActive && !this.IsJudged)
        {
            var message = msg as SimpleGestureMessage;
            if (message.hit.collider == this.collider2D)
            {
                this.state = NoteState.Judged;
                double current_time = GameMgr.Instance.CurrentTime;
                //Debug.Log("tap: " + current_time + " cfg_time: " + cfg.time);

                ScoreMgr.ScoreLevel level = GameMgr.Instance.scoreMgr.JudgeClickTime(this.JudgeIntervalConfig, current_time, cfg.FirstCheckPoint().time);
                // 计分
                Debug.Log("[判定] 类型: Tap, 结果: " + level);
                if (level != ScoreMgr.ScoreLevel.perfect)
                    GameMgr.Instance.scoreMgr.CountEarlyOrLate(current_time, cfg.FirstCheckPoint().time);
                GameMgr.Instance.AddScore(level);
                // 点击效果
                PlayEffect(level);
                //
                NotePoolManager.Instance.ReturnObject(this);
            }

        }
    }
}
