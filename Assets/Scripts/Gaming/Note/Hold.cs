using UnityEngine;
using Note;
using Music;
using System.Collections.Generic;
using GestureEvent;
using NoteGesture;

/// <summary>
/// 长按
/// 首判：正常判定，如果是bad则该note直接判为bad
/// 尾判: 不提前松手
/// </summary>
public class Hold : NoteBase
{
    private HoldPolygonImage touch_area;
    private HoldPolygonImage icon;
    private RectTransform head_handle;
    private RectTransform tail_handle;
    private new PolygonCollider2D collider2D;

    double start_time;
    double end_time;
    ScoreMgr.ScoreLevel start_judge_level;
    ScoreMgr.ScoreLevel end_judge_level;

    float hold_effect_time;
    float hold_effect_cd = 0.2f;

    bool is_finger_down;
    List<CheckPoint> checkPoints_cache;
    private HashSet<int> hold_fingers = new HashSet<int>();
    private int FingerCount => hold_fingers.Count;
    private bool IsHolding => FingerCount > 0;

    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Hold;
        touch_area = this.GetComponent<HoldPolygonImage>();
        collider2D = this.GetComponent<PolygonCollider2D>();
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

    public void OnPointerDown(IGestureMessage msg)
    {
        var message = msg as SimpleGestureMessage;
        if (!touch_area.IsRaycastLocationValid(message.position, Camera.main)) return;
        hold_fingers.Add(message.fingerId);
        if (message.hit.collider != this.collider2D) return;
        if (this.IsActive)
        {
            if (!is_finger_down)
            {
                is_finger_down = true;
                start_time = GameMgr.Instance.CurrentTime;
                Debug.Log("Hold start " + start_time);

                start_judge_level = GameMgr.Instance.scoreMgr.JudgeClickTime(this.JudgeIntervalConfig, start_time, cfg.FirstCheckPoint().time);
                PlayEffect(start_judge_level);
                if (start_judge_level != ScoreMgr.ScoreLevel.perfect)
                    GameMgr.Instance.scoreMgr.CountEarlyOrLate(start_time, cfg.FirstCheckPoint().time);
                if (start_judge_level == ScoreMgr.ScoreLevel.bad)
                {
                    state = NoteState.Judged;
                    Debug.Log("[判定] 类型: Hold, 结果: " + start_judge_level);
                    GameMgr.Instance.AddScore(ScoreMgr.ScoreLevel.bad);
                    NotePoolManager.Instance.ReturnObject(this);
                }
            }
        }
    }

    public void OnPointerStayOrMove(IGestureMessage msg)
    {
        var message = msg as SimpleGestureMessage;
        bool isHit = touch_area.IsRaycastLocationValid(message.position, Camera.main);
        if (hold_fingers.Contains(message.fingerId))
        {
            if (isHit) return;
            // finger exit the touch area
            hold_fingers.Remove(message.fingerId);
            if (this.IsActive)
            {
                if (is_finger_down && !IsHolding && is_move)
                {
                    EndJudge();
                }
            }
        }
        else
        {
            if (!isHit) return;
            // finger enter the touch area
            hold_fingers.Add(message.fingerId);
        }
    }

    public void OnPointerUp(IGestureMessage msg)
    {
        var message = msg as SimpleGestureMessage;
        if (!hold_fingers.Contains(message.fingerId)) return;
        hold_fingers.Remove(message.fingerId);
        if (this.IsActive)
        {
            if (is_finger_down && !IsHolding && is_move)
            {
                EndJudge();
            }
        }
    }

    public override void Init(NoteCfg _cfg, float delta_time, GestureEvent.GestureMgr gestureMgr)
    {
        if (checkPoints_cache == null) checkPoints_cache = new List<CheckPoint>();
        checkPoints_cache.Clear();
        foreach (CheckPoint ckp in _cfg.checkPoints)
        {
            checkPoints_cache.Add(new CheckPoint(ckp));
        }

        base.Init(_cfg, delta_time, gestureMgr);
        is_finger_down = false;
        hold_fingers.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Resize()
    {
        float drop_speed = PlayerPersonalSetting.ScaledDropSpeed;
        icon.SetCheckPoints(checkPoints_cache, drop_speed, Screen.width);
        touch_area.SetCheckPoints(checkPoints_cache, drop_speed, Screen.width, GameConst.hold_touch_area_width_extend, JudgeIntervalConfig.active_interval);
        ReshapeCollider(touch_area.mesh_points);
        head_handle.localPosition = icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, icon.TailWidth / tail_handle.localScale.x);
    }

    protected override void ResetPosition(float delta_time)
    {
        float x = 0;
        float y = icon.Height / 2 + delta_time * PlayerPersonalSetting.ScaledDropSpeed;
        rectTransform.localPosition = new Vector2(x, y);
    }


    protected override void EndJudge()
    {
        if (is_finger_down)
        {
            state = NoteState.Judged;
            end_time = GameMgr.Instance.CurrentTime;
            end_judge_level = GameMgr.Instance.scoreMgr.JudgeHoldEnd(this.JudgeIntervalConfig, end_time, cfg.LastCheckPoint().time);

            Debug.Log("Hold end " + end_time);
            ScoreMgr.ScoreLevel level;
            if (start_judge_level == ScoreMgr.ScoreLevel.perfect && end_judge_level == ScoreMgr.ScoreLevel.perfect)
                level = ScoreMgr.ScoreLevel.perfect;
            else if (start_judge_level == ScoreMgr.ScoreLevel.bad || end_judge_level == ScoreMgr.ScoreLevel.bad)
                level = ScoreMgr.ScoreLevel.bad;
            else
                level = ScoreMgr.ScoreLevel.good;

            Debug.Log("[判定] 类型: Hold, 结果: " + level);
            if (end_judge_level != ScoreMgr.ScoreLevel.perfect)
                GameMgr.Instance.scoreMgr.CountEarlyOrLate(end_time, cfg.LastCheckPoint().time);
            GameMgr.Instance.AddScore(level);
            PlayEffect(level);
            NotePoolManager.Instance.ReturnObject(this);
        }
    }

    private void PlayEffectOnHolding()
    {
        if (tail_handle.position.y > GameMgr.Instance.JudgeLinePositionY)
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
        if (!is_finger_down  && cfg.FirstCheckPoint().time + this.JudgeIntervalConfig.good_interval < GameMgr.Instance.CurrentTime)
        {
            Miss();
        }
    }

    private void CheckLastCheckPoint()
    {
        if (is_finger_down && FingerCount > 0 && cfg.LastCheckPoint().time < GameMgr.Instance.CurrentTime)
        {
            EndJudge();
        }
    }

    private void ReshapeCollider(Vector3[] mesh_points)
    {
        Vector2[] colliderPoints = new Vector2[mesh_points.Length];
        int index = 0;
        for (int i=0; i<mesh_points.Length / 2; i++)
        {
            colliderPoints[index].x = mesh_points[i * 2].x;
            colliderPoints[index].y = mesh_points[i * 2].y;
            index++;    

        }
        for (int i= mesh_points.Length / 2 - 1; i>=0; i--)
        {
            colliderPoints[index].x = mesh_points[i * 2 + 1].x;
            colliderPoints[index].y = mesh_points[i * 2 + 1].y;
            index++;
        }
        collider2D.SetPath(0, colliderPoints);
    }

    private void ModifyShapeOnReachJudgeLine()
    {
        //Debug.Log("[ModifyShapeOnReachJudgeLine]" + head_handle.position.y + " " + JudgeLine.PositionY);
        if (head_handle.position.y < GameMgr.Instance.JudgeLinePositionY)
        {
            if (GameMgr.Instance.CurrentTime > checkPoints_cache[1].time)
            {
                if (checkPoints_cache.Count > 2)
                    checkPoints_cache.RemoveAt(0);
            }
            checkPoints_cache[0] = GetCheckPointOnJudgeLine;
            Resize();
            if (tail_handle.position.y > GameMgr.Instance.JudgeLinePositionY)
            {
                rectTransform.anchoredPosition = Util.ChangeV3(rectTransform.anchoredPosition, 1, GameMgr.Instance.JudgeLineLocalPositionY + icon.Height * 0.5f);
            }
        }
    }

    protected override float TouchAreaLength
    {
        get
        {
            return PlayerPersonalSetting.ScaledDropSpeed * (JudgeIntervalConfig.active_interval * 2 + (float)cfg.Duration());
        }
    }

    protected override float GetCenterXOnJudgeLine
    {
        get
        {
            return ((float)GetCheckPointOnJudgeLine.Center() - 0.5f) * Screen.width;
        }
    }

    protected override void RegisterGestureHandler()
    {
        gestureMgr.AddListener<BeganGestureRecognizer>(OnPointerDown);
        gestureMgr.AddListener<StationaryGestureRecognizer>(OnPointerStayOrMove);
        gestureMgr.AddListener<MoveGestureRecognizer>(OnPointerStayOrMove);
        gestureMgr.AddListener<EndGestureRecognizer>(OnPointerUp);
    }

    protected override void UnregisterGestureHandler()
    {
        gestureMgr.RemoveListener<BeganGestureRecognizer>(OnPointerDown);
        gestureMgr.RemoveListener<StationaryGestureRecognizer>(OnPointerStayOrMove);
        gestureMgr.RemoveListener<MoveGestureRecognizer>(OnPointerStayOrMove);
        gestureMgr.RemoveListener<EndGestureRecognizer>(OnPointerUp);
    }
}
