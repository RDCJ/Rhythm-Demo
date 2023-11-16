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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active)
        {
            is_holding = true;
            start_time = GameMgr.Instance.current_time;
            Debug.Log("Hold start " + start_time);

            start_judge_level = ScoreMgr.JudgeClickTime(start_time, cfg.time); ;
            
            if (start_judge_level == ScoreMgr.ScoreLevel.bad)
            {
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
            Debug.Log("Hold end " + end_time);
            EndJudge();
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (is_holding)
        {
            is_holding = false;
            Debug.Log("Hold end " + end_time);
            EndJudge();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        type = NoteType.Hold;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Init(NoteCfg _cfg)
    {
        base.Init(_cfg);
        is_holding = false;
        Resize();
    }

    private void Resize()
    {
        float x = rectTransform.sizeDelta.x;
        float y = (float)cfg.duration * GameConst.drop_speed;
        rectTransform.sizeDelta = new Vector2(x, y);
    }


    private void EndJudge()
    {
        end_time = GameMgr.Instance.current_time;
        end_judge_level = ScoreMgr.JudgeClickTime(end_time, cfg.time + cfg.duration);

        ScoreMgr.ScoreLevel level;
        if (start_judge_level == ScoreMgr.ScoreLevel.perfect && end_judge_level == ScoreMgr.ScoreLevel.perfect)
            level = ScoreMgr.ScoreLevel.perfect;
        else if (start_judge_level == ScoreMgr.ScoreLevel.bad || end_judge_level == ScoreMgr.ScoreLevel.bad)
            level = ScoreMgr.ScoreLevel.bad;
        else
            level = ScoreMgr.ScoreLevel.good;

        float x = this.transform.position.x;
        float y = JudgeLine.Instance.transform.position.y;

        ScoreMgr.Instance.AddScore(level);
        EffectPlayer.Instance.PlayEffect(level, new Vector3(x, y, 0));

        NotePoolManager.Instance.ReturnObject(this);
    }

}
