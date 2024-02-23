using UnityEngine;
using UnityEngine.EventSystems;
using Note;

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

    float hold_effect_time;
    float hold_effect_cd = 0.2f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.IsActive)
        {
            finger_count++;
            start_time = GameMgr.Instance.current_time;
            Debug.Log("Hold start " + start_time);

            start_judge_level = ScoreMgr.Instance.JudgeClickTime(start_time, cfg.FirstCheckPoint().time);
            PlayEffect(start_judge_level);

            ScoreMgr.Instance.CountEarlyOrLate(start_time, cfg.FirstCheckPoint().time);
            if (start_judge_level == ScoreMgr.ScoreLevel.bad)
            {
                state = NoteState.Judged;
                Debug.Log("[判定] 类型: Hold, 结果: " + start_judge_level);
                ScoreMgr.Instance.AddScore(ScoreMgr.ScoreLevel.bad);
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

    protected override void Update()
    {
        base.Update();
        if (IsHolding)
        {
            hold_effect_time -= Time.deltaTime;
            if (hold_effect_time < 0)
            {
                PlayEffect(start_judge_level);
                hold_effect_time = hold_effect_cd;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Resize()
    {
        float drop_speed = DropSpeedFix.GetScaledDropSpeed;
        icon.SetCheckPoints(cfg.checkPoints, drop_speed, Screen.width);
        touch_area.SetCheckPoints(cfg.checkPoints, drop_speed, Screen.width, GameConst.hold_touch_area_width_extend, GameConst.active_interval);

        head_handle.localPosition = icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, icon.TailWidth / tail_handle.localScale.x);
    }

    protected override void ResetPosition(float delta_time)
    {
        float x = 0;
        float y = Screen.height / 2 + icon.Height / 2 + delta_time * DropSpeedFix.GetScaledDropSpeed;
        rectTransform.localPosition = new Vector2(x, y);
    }

    private void EndJudge()
    {
        state = NoteState.Judged;
        end_time = GameMgr.Instance.current_time;
        end_judge_level = ScoreMgr.Instance.JudgeHoldEnd(end_time, cfg.LastCheckPoint().time);

        Debug.Log("Hold end " + end_time);
        ScoreMgr.ScoreLevel level;
        if (start_judge_level == ScoreMgr.ScoreLevel.perfect && end_judge_level == ScoreMgr.ScoreLevel.perfect)
            level = ScoreMgr.ScoreLevel.perfect;
        else if (start_judge_level == ScoreMgr.ScoreLevel.bad || end_judge_level == ScoreMgr.ScoreLevel.bad)
            level = ScoreMgr.ScoreLevel.bad;
        else
            level = ScoreMgr.ScoreLevel.good;

        Debug.Log("[判定] 类型: Hold, 结果: " + level);
        ScoreMgr.Instance.CountEarlyOrLate(end_time, cfg.LastCheckPoint().time);
        ScoreMgr.Instance.AddScore(level);
        PlayEffect(level);
        NotePoolManager.Instance.ReturnObject(this);
    }


    protected override float TouchAreaLength
    {
        get
        {
            return DropSpeedFix.GetScaledDropSpeed * (GameConst.active_interval * 2 + (float)cfg.Duration());
        }
    }


    protected override float GetCenterXOnJudgeLine
    {
        get
        {
            double time = GameMgr.Instance.current_time;
            int n = cfg.checkPoints.Count;
            if (time < cfg.FirstCheckPoint().time)
                return (float)cfg.FirstCheckPoint().Center() * Screen.width;
            for (int i = 0; i < n - 1; i++)
            {
                var ckp1 = cfg.checkPoints[i];
                var ckp2 = cfg.checkPoints[i + 1];
                if (time >= ckp1.time && time <= ckp2.time)
                {
                    double k = (time - ckp1.time) / (ckp2.time - ckp1.time);
                    double l = ckp1.position_l + k * (ckp2.position_l - ckp1.position_l);
                    double r = ckp1.position_r + k * (ckp2.position_r - ckp1.position_r);
                    return (float)(l + r) / 2 * Screen.width;
                }
            }
            return (float)cfg.LastCheckPoint().Center() * Screen.width;
        }
        
    }
}
