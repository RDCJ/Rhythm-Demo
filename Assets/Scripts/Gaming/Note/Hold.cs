using UnityEngine;
using UnityEngine.EventSystems;
using Note;
using Music;
using System.Collections.Generic;

/// <summary>
/// 长按
/// 首判：正常判定，如果是bad则该note直接判为bad
/// 尾判: 不提前松手
/// </summary>
public class Hold : NoteBase, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
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

    bool is_finger_down;
    List<CheckPoint> checkPoints_cache;

    public void OnPointerDown(PointerEventData eventData)
    {
        finger_count++;
        if (this.IsActive)
        {
            if (!is_finger_down)
            {
                is_finger_down = true;
                start_time = GameMgr.Instance.CurrentTime;
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        finger_count++;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        finger_count--;
        if (this.IsActive)
        {
            if (is_finger_down && !IsHolding && is_move)
            {
                EndJudge();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        finger_count--;
        if (this.IsActive)
        {
            if (is_finger_down && !IsHolding && is_move)
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
        if (is_move)
        {
           CheckFirstCheckPointMiss();

            CheckLastCheckPoint();

            PlayEffectOnHolding();

            ModifyShapeOnReachJudgeLine();
        }
    }

    public override void Init(NoteCfg _cfg, float delta_time)
    {
        if (checkPoints_cache == null) checkPoints_cache = new List<CheckPoint>();
        checkPoints_cache.Clear();
        foreach (CheckPoint ckp in _cfg.checkPoints)
        {
            checkPoints_cache.Add(new CheckPoint(ckp));
        }

        base.Init(_cfg, delta_time);
        is_finger_down = false;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Resize()
    {
        float drop_speed = DropSpeedFix.GetScaledDropSpeed;
        icon.SetCheckPoints(checkPoints_cache, drop_speed, Screen.width);
        touch_area.SetCheckPoints(checkPoints_cache, drop_speed, Screen.width, GameConst.hold_touch_area_width_extend, GameConst.active_interval);

        head_handle.localPosition = icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, icon.TailWidth / tail_handle.localScale.x);
    }

    protected override void ResetPosition(float delta_time)
    {
        float x = 0;
        float y = icon.Height / 2 + delta_time * DropSpeedFix.GetScaledDropSpeed;
        rectTransform.localPosition = new Vector2(x, y);
    }


    protected override void EndJudge()
    {
        if (is_finger_down)
        {
            state = NoteState.Judged;
            end_time = GameMgr.Instance.CurrentTime;
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
    }

    private void PlayEffectOnHolding()
    {
        if (tail_handle.position.y > JudgeLine.PositionY)
        {
            if (IsHolding && is_finger_down)
            {
                hold_effect_time -= Time.deltaTime;
                if (hold_effect_time < 0)
                {
                    PlayEffect(start_judge_level);
                    hold_effect_time = hold_effect_cd;
                }
            }
        }
    }

    private void CheckFirstCheckPointMiss()
    {
        if (!is_finger_down  && cfg.FirstCheckPoint().time + GameConst.good_interval < GameMgr.Instance.CurrentTime)
        {
            Miss();
        }
    }

    private void CheckLastCheckPoint()
    {
        if (is_finger_down && finger_count > 0 && cfg.LastCheckPoint().time < GameMgr.Instance.CurrentTime)
        {
            EndJudge();
        }
    }

    private void ModifyShapeOnReachJudgeLine()
    {
        //Debug.Log("[ModifyShapeOnReachJudgeLine]" + head_handle.position.y + " " + JudgeLine.PositionY);
        if (head_handle.position.y < JudgeLine.PositionY)
        {
            if (GameMgr.Instance.CurrentTime > checkPoints_cache[1].time)
            {
                if (checkPoints_cache.Count > 2)
                    checkPoints_cache.RemoveAt(0);
            }
            checkPoints_cache[0] = GetCheckPointOnJudgeLine;
            Resize();
            if (tail_handle.position.y > JudgeLine.PositionY)
            {
                rectTransform.anchoredPosition = Util.ChangeV3(rectTransform.anchoredPosition, 1, JudgeLine.localPositionY + icon.Height * 0.5f);
            }
        }
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
            return ((float)GetCheckPointOnJudgeLine.Center() - 0.5f) * Screen.width;
        }
    }
}
