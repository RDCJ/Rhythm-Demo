using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using Music;

/// <summary>
/// 长按
/// 首判：正常判定，如果是bad则该note直接判为bad
/// 尾判: 不提前松手
/// </summary>
public class Hold : NoteBase, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private HoldPolygonImage touch_area;
    private HoldPolygonImage icon;
    private RectTransform head_handle;
    private RectTransform tail_handle;

    double start_time;
    double end_time;
    ScoreMgr.ScoreLevel start_judge_level;
    ScoreMgr.ScoreLevel end_judge_level;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.IsActive)
        {
            finger_count++;
            start_time = GameMgr.Instance.current_time;
            Debug.Log("Hold start " + start_time);

            start_judge_level = ScoreMgr.Instance.JudgeClickTime(start_time, cfg.FirstCheckPoint().time); ;
            PlayEffect(start_judge_level);

            if (start_judge_level == ScoreMgr.ScoreLevel.bad)
            {
                state = NoteState.Judged;
                Debug.Log("[判定] 类型: Hold, 结果: " + start_judge_level);
                NotePoolManager.Instance.ReturnObject(this);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.IsActive)
        {
            finger_count--;
            if (!IsHolding)
            {
                EndJudge();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.IsActive)
        {
            finger_count--;
            if (!IsHolding)
            {
                EndJudge();
            }
        }
    }


    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Hold;
        touch_area = this.GetComponent<HoldPolygonImage>();
        icon = transform.Find("icon").GetComponent<HoldPolygonImage>();
        head_handle = transform.Find("icon/head_handle") as RectTransform;
        tail_handle = transform.Find("icon/tail_handle") as RectTransform;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Resize()
    {
        float drop_speed = DropSpeedFix.GetScaledDropSpeed / MainCanvas.Instance.GetScaleFactor;
        icon.SetCheckPoints(cfg.checkPoints, drop_speed, Screen.width);
        touch_area.SetCheckPoints(cfg.checkPoints, drop_speed, Screen.width, GameConst.hold_touch_area_width_extend, GameConst.active_interval);

        head_handle.localPosition = icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, icon.TailWidth / tail_handle.localScale.x);
    }

    protected override void ResetPosition(float delta_time)
    {
        float y = Screen.height + icon.Height * MainCanvas.Instance.GetScaleFactor / 2 + delta_time * DropSpeedFix.GetScaledDropSpeed;
        rectTransform.position = Util.ChangeV3(rectTransform.position, 1, y);
        rectTransform.localPosition = Util.ChangeV3(rectTransform.localPosition, 0, 0);
    }

    private void EndJudge()
    {
        state = NoteState.Judged;
        end_time = GameMgr.Instance.current_time;
        end_judge_level = ScoreMgr.Instance.JudgeHoldEnd(end_time, cfg.FirstCheckPoint().time + cfg.Duration());

        Debug.Log("Hold end " + end_time);
        ScoreMgr.ScoreLevel level;
        if (start_judge_level == ScoreMgr.ScoreLevel.perfect && end_judge_level == ScoreMgr.ScoreLevel.perfect)
            level = ScoreMgr.ScoreLevel.perfect;
        else if (start_judge_level == ScoreMgr.ScoreLevel.bad || end_judge_level == ScoreMgr.ScoreLevel.bad)
            level = ScoreMgr.ScoreLevel.bad;
        else
            level = ScoreMgr.ScoreLevel.good;

        Debug.Log("[判定] 类型: Hold, 结果: " + level);
        ScoreMgr.Instance.AddScore(level);
        PlayEffect(level);
        NotePoolManager.Instance.ReturnObject(this);
    }


    protected override float TouchAreaLength
    {
        get
        {
            return DropSpeedFix.GetScaledDropSpeed * (GameConst.active_interval * 2 + (float)cfg.Duration()) / MainCanvas.Instance.GetScaleFactor;
        }
    }
}
