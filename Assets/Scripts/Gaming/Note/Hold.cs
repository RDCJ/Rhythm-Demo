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
    double start_time;
    double end_time;
    ScoreMgr.ScoreLevel start_judge_level;
    ScoreMgr.ScoreLevel end_judge_level;
    HoldIcon icon;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.IsActive)
        {
            finger_count++;
            start_time = GameMgr.Instance.current_time;
            Debug.Log("Hold start " + start_time);

            start_judge_level = ScoreMgr.Instance.JudgeClickTime(start_time, cfg.time); ;
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
        icon= transform.Find("icon").GetComponent<HoldIcon>();
    }

    public override void Init(NoteCfg _cfg, float delta_time)
    {
        base.Init(_cfg, delta_time);
    }

    /// <summary>
    /// 根据下落时间调整长度，canvas scaler对实际的长度有影响
    /// </summary>
    protected override void Resize()
    {
        float touch_area_length = DropSpeedFix.GetScaledDropSpeed * (GameConst.active_interval * 2 + (float)cfg.duration) / MainCanvas.Instance.GetScaleFactor;
        float icon_length = DropSpeedFix.GetScaledDropSpeed * (float)cfg.duration / MainCanvas.Instance.GetScaleFactor;
        rectTransform.sizeDelta = Util.ChangeV2(rectTransform.sizeDelta, 1, touch_area_length);
        collider.size = rectTransform.sizeDelta;

        icon.Resize(icon_length);
    }

    protected override void ResetPosition(float delta_time)
    {
        float x = (float)cfg.position_x * Screen.width;
        float y = Screen.height + icon.sizeDelta.y * MainCanvas.Instance.GetScaleFactor / 2 + delta_time * DropSpeedFix.GetScaledDropSpeed;
        rectTransform.position = new Vector3(x, y, 0);
    }

    private void EndJudge()
    {
        state = NoteState.Judged;
        end_time = GameMgr.Instance.current_time;
        end_judge_level = ScoreMgr.Instance.JudgeHoldEnd(end_time, cfg.time + cfg.duration);

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

}
