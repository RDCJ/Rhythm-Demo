using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using Music;

/// <summary>
/// ������û��β��
/// </summary>
public class Hold : NoteBase, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    bool is_holding;
    double start_time;
    double end_time;
    ScoreMgr.ScoreLevel start_judge_level;
    ScoreMgr.ScoreLevel end_judge_level;
    HoldIcon icon;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active)
        {
            is_holding = true;
            start_time = GameMgr.Instance.current_time;
            Debug.Log("Hold start " + start_time);

            start_judge_level = ScoreMgr.Instance.JudgeClickTime(start_time, cfg.time); ;
            PlayEffect(start_judge_level);

            if (start_judge_level == ScoreMgr.ScoreLevel.bad)
            {
                is_judged = true;
                Debug.Log("[�ж�] ����: Hold, ���: " + start_judge_level);
                NotePoolManager.Instance.ReturnObject(this);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            EndJudge();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            EndJudge();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Hold;
        icon= transform.Find("icon").GetComponent<HoldIcon>();
    }

/*    protected override void Update()
    {
        base.Update();
        *//*        if (is_move)
                {
                    icon.IconUpdate();
                }*//*
        Debug.Log(rectTransform.position);
    }
*/
    public override void Init(NoteCfg _cfg, float delta_time)
    {
        base.Init(_cfg, delta_time);
        is_holding = false;
    }

    /// <summary>
    /// ��������ʱ��������ȣ�canvas scaler��ʵ�ʵĳ�����Ӱ��
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
        is_judged = true;
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

        Debug.Log("[�ж�] ����: Hold, ���: " + level);
        ScoreMgr.Instance.AddScore(level);
        PlayEffect(level);
        NotePoolManager.Instance.ReturnObject(this);
    }

}
