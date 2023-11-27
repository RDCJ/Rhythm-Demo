using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using Music;

public class Hold : NoteBase, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    bool is_holding;
    double start_time;
    double end_time;
    ScoreMgr.ScoreLevel start_judge_level;
    ScoreMgr.ScoreLevel end_judge_level;
    RectTransform icon_rect;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active)
        {
            is_holding = true;
            start_time = GameMgr.Instance.current_time;
            //Debug.Log("Hold start " + start_time);

            start_judge_level = ScoreMgr.Instance.JudgeClickTime(start_time, cfg.time); ;
            
            if (start_judge_level == ScoreMgr.ScoreLevel.bad)
            {
                is_judged = true;
                Debug.Log("[判定] 类型: Hold, 结果: " + start_judge_level);
                float x = this.transform.position.x;
                float y = JudgeLine.Instance.transform.position.y;
                EffectPlayer.Instance.PlayEffect(start_judge_level, new Vector3(x, y, 0));
                NotePoolManager.Instance.ReturnObject(this);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            //Debug.Log("Hold end " + end_time);
            EndJudge();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            //Debug.Log("Hold end " + end_time);
            EndJudge();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Hold;
        icon_rect = transform.Find("icon").GetComponent<RectTransform>();
    }

    public override void Init(NoteCfg _cfg, float delta_time)
    {
        base.Init(_cfg, delta_time);
        is_holding = false;
    }

    protected override void ResetPosition(float delta_time)
    {
        float x = (float)cfg.position_x * Screen.width;
        float y = Screen.height + icon_rect.sizeDelta.y / 2 + delta_time * DropSpeedFix.GetScaledDropSpeed;
        rectTransform.position = new Vector2(x, y);
    }

    protected override void Resize()
    {
        Vector2 v = rectTransform.sizeDelta;
        v.y = DropSpeedFix.GetScaledDropSpeed * (GameConst.active_interval * 2 + (float)cfg.duration);
        rectTransform.sizeDelta = v;
        collider.size = rectTransform.sizeDelta;

        v.y = DropSpeedFix.GetScaledDropSpeed * (float)cfg.duration;
        icon_rect.sizeDelta = v;
    }


    private void EndJudge()
    {
        is_judged = true;
        end_time = GameMgr.Instance.current_time;
        end_judge_level = ScoreMgr.Instance.JudgeHoldEnd(end_time, cfg.time + cfg.duration);

        ScoreMgr.ScoreLevel level;
        if (start_judge_level == ScoreMgr.ScoreLevel.perfect && end_judge_level == ScoreMgr.ScoreLevel.perfect)
            level = ScoreMgr.ScoreLevel.perfect;
        else if (start_judge_level == ScoreMgr.ScoreLevel.bad || end_judge_level == ScoreMgr.ScoreLevel.bad)
            level = ScoreMgr.ScoreLevel.bad;
        else
            level = ScoreMgr.ScoreLevel.good;
        
        float x = this.transform.position.x;
        float y = JudgeLine.Instance.transform.position.y;

        Debug.Log("[判定] 类型: Hold, 结果: " + level);
        ScoreMgr.Instance.AddScore(level);
        EffectPlayer.Instance.PlayEffect(level, new Vector3(x, y, 0));

        NotePoolManager.Instance.ReturnObject(this);
    }

}
