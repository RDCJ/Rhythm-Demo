using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using Music;


/// <summary>
/// 点击并滑动，滑动时正常判定
/// </summary>
public class Slide : NoteBase, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum SlideDirection
    {
        Left,
        Right
    }
    public SlideDirection direciton;

    private bool is_down;
    private Vector2 down_position;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active)
        {
            is_down = true;
            down_position = eventData.position;
            //Debug.Log("slide down " + down_position);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (is_down && !is_judged)
        {
            is_judged = true;
            Vector2 move_position = eventData.position;
            Vector2 slide_direction = move_position - down_position;
            ScoreMgr.ScoreLevel level;

            if ((slide_direction.x < 0) == (direciton == SlideDirection.Left))
            {
                level = ScoreMgr.Instance.JudgeClickTime(GameMgr.Instance.current_time, cfg.time);
                //Debug.Log("sliding right " + eventData.position);
            }
            else
            {
                level = ScoreMgr.ScoreLevel.bad;
                //Debug.Log("sliding wrong " + eventData.position);
            }
            Debug.Log("[判定] 类型: Slide, 结果: " + level);
            ScoreMgr.Instance.AddScore(level);
            PlayEffect(level);

            NotePoolManager.Instance.ReturnObject(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        is_down = false;
        Debug.Log("slide up");
    }

    protected override void Awake()
    {
        base.Awake();
        if (direciton == SlideDirection.Left)
            type = NoteType.LeftSlide;
        else
            type = NoteType.RightSlide;
    }

    public override void Init(NoteCfg _cfg, float delta_time)
    {
        base.Init(_cfg, delta_time);
        is_down = false;
    }
}
