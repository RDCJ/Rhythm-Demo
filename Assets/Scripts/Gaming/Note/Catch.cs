using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Note;

/// <summary>
/// 滑动，判定框小，必判perfect
/// </summary>
public class Catch : NoteBase, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler
{
    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Catch;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.IsActive && !this.IsJudged)
        {
            Judge();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (this.IsActive && !this.IsJudged)
        {
            Judge();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.IsActive && !this.IsJudged)
        {
            Judge();
        }
    }

    private void Judge()
    {
        state = NoteState.Judged;

        ScoreMgr.ScoreLevel level = ScoreMgr.ScoreLevel.perfect;
        // 计分
        Debug.Log("[判定] 时间: " + GameMgr.Instance.CurrentTime.ToString("N4") + "类型: Catch, 结果: " + level);
        ScoreMgr.Instance.AddScore(level);
        // 点击效果
        PlayEffect(level);
        //
        NotePoolManager.Instance.ReturnObject(this);
    }

    protected override float TouchAreaLength
    {
        get
        {
            return DropSpeedFix.GetScaledDropSpeed * GameConst.good_interval * 2;
        }
    }
       
}
