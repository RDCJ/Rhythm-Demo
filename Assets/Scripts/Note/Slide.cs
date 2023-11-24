using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Note;
using Music;

public class Slide : NoteBase, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum SlideDirection
    {
        Left,
        Right
    }
    public SlideDirection direciton;

    private bool is_down;
    private bool is_slided;
    private Vector2 down_position;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.is_active)
        {
            is_down = true;
            down_position = eventData.position;
            Debug.Log("slide down " + down_position);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (is_down && !is_slided)
        {
            is_slided = true;
            Vector2 move_position = eventData.position;
            Vector2 slide_direction = move_position - down_position;
            ScoreMgr.ScoreLevel level;
            float x = this.transform.position.x;
            float y = JudgeLine.Instance.transform.position.y;

            if ((slide_direction.x < 0) == (direciton == SlideDirection.Left))
            {
                level = ScoreMgr.JudgeClickTime(GameMgr.Instance.current_time, cfg.time);
                Debug.Log("sliding right " + eventData.position);
            }
            else
            {
                level = ScoreMgr.ScoreLevel.bad;
                Debug.Log("sliding wrong " + eventData.position);
            }
                
            ScoreMgr.Instance.AddScore(level);
            EffectPlayer.Instance.PlayEffect(level, new Vector3(x, y, 0));

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
        is_slided = false;
    }
}
