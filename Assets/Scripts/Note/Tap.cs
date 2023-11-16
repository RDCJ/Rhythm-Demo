using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;

public class Tap : NoteBase, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active)
        {
            double current_time = GameMgr.Instance.current_time;
            Debug.Log("tap: " + current_time + " cfg_time: " + cfg.time);

            ScoreMgr.ScoreLevel level = ScoreMgr.JudgeClickTime(current_time, cfg.time);
            // 计分
            ScoreMgr.Instance.AddScore(level);
            // 点击效果
            float x = this.transform.position.x;
            float y = JudgeLine.Instance.transform.position.y;
            EffectPlayer.Instance.PlayEffect(level, new Vector3(x, y, 0));
            //
            NotePoolManager.Instance.ReturnObject(this);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Tap;
    }
}
