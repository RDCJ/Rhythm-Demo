using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;

/// <summary>
/// 点击，正常判定
/// </summary>
public class Tap : NoteBase, IPointerDownHandler
{
    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Tap;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.IsActive)
        {
            this.state = NoteState.Judged;
            double current_time = GameMgr.Instance.current_time;
            //Debug.Log("tap: " + current_time + " cfg_time: " + cfg.time);

            ScoreMgr.ScoreLevel level = ScoreMgr.Instance.JudgeClickTime(current_time, cfg.time);
            // 计分
            Debug.Log("[判定] 类型: Tap, 结果: " + level);
            ScoreMgr.Instance.AddScore(level);
            // 点击效果
            PlayEffect(level);
            //
            NotePoolManager.Instance.ReturnObject(this);
        }
    }
}
