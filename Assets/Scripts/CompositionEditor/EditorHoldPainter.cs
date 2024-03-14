using Music;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorHoldPainter : EditorNotePainter
{
    GameObject checkPointDrag;
    HoldPolygonImage hold_icon;

    List<CheckPointDrag> center_drags;
    List<CheckPointDrag> left_drags;
    List<CheckPointDrag> right_drags;

    protected override void Awake()
    {
        base.Awake();
        hold_icon = transform.Find("icon").GetComponent<HoldPolygonImage>();
        checkPointDrag = this.transform.Find("checkpoint_drag").gameObject;
        center_drags = new List<CheckPointDrag>();
        left_drags = new List<CheckPointDrag>();
        right_drags = new List<CheckPointDrag>();
    }

    public override void OnPaint()
    {
        RePaint();
        SetCheckPointDrag();
    }

    private void RePaint()
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        PaintHold(hold_icon.transform as RectTransform, cfg);

        float x = 0;
        float y = drop_speed * (float)(cfg.FirstCheckPoint().time - CompositionEditor.Instance.GetTimeOffset) - CompositionDisplay.Instance.ContentHeight;
        y += hold_icon.rectTransform.sizeDelta.y * hold_icon.rectTransform.localScale.y / 2.0f;

        rectTransform.anchoredPosition = Util.ChangeV2(rectTransform.anchoredPosition, 0, x);
        rectTransform.localPosition = Util.ChangeV3(rectTransform.localPosition, 1, y);
    }


    public static void PaintHold(RectTransform note_trans, NoteCfg cfg)
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        var hold_icon = note_trans.GetComponent<HoldPolygonImage>();
        hold_icon.SetCheckPoints(cfg.checkPoints, drop_speed, CompositionDisplay.Instance.WindowWidth);

        var head_handle = note_trans.Find("head_handle") as RectTransform;
        var tail_handle = note_trans.Find("tail_handle") as RectTransform;
        head_handle.localPosition = hold_icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, hold_icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = hold_icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, hold_icon.TailWidth / tail_handle.localScale.x);
    }

    private void SetCheckPointDrag()
    {
        void SetDrag(List<CheckPointDrag> list, CheckPointDrag.TYPE type, int index)
        {
            CheckPointDrag drag;
            if (list.Count < index)
            {
                drag = Instantiate(checkPointDrag, transform).GetComponent<CheckPointDrag>();
                drag.index = index - 1;
                drag.type = type;
                list.Add(drag);
            }
            else
                drag = list[index - 1];

            drag.gameObject.SetActive(true);
            RectTransform drag_tf = drag.rectTransform;
            switch (type)
            {
                case CheckPointDrag.TYPE.Center:
                    drag_tf.localPosition = hold_icon.GetCheckPointCenter(index);
                    break;
                 case CheckPointDrag.TYPE.Left:
                    drag_tf.localPosition = hold_icon.GetCheckPointLeft(index);
                    break;
                 case CheckPointDrag.TYPE.Right:
                    drag_tf.localPosition = hold_icon.GetCheckPointRight(index);
                    break;
            }
            
        }

        for (int i = 1; i <= hold_icon.checkpoint_count; i++)
        {
            SetDrag(center_drags, CheckPointDrag.TYPE.Center, i);
            SetDrag(left_drags, CheckPointDrag.TYPE.Left, i);
            SetDrag(right_drags, CheckPointDrag.TYPE.Right, i);
        }
    }

    public void OnCheckPointDragCenter(PointerEventData eventData, int index)
    {
        double time = HorizontalGridLine.Instance.GetNearestTime(eventData.position.y);
        if (index > 0)
        {
            double last_time = cfg.checkPoints[index - 1].time;
            if (time <= last_time) return;
        }
        if (index < cfg.checkPoints.Count - 1)
        {
            double next_time = cfg.checkPoints[cfg.checkPoints.Count - 1].time;
            if (time >= next_time) return;
        }
        double new_center = VerticalGridLine.Instance.GetPositionX(eventData, false);
        double width = cfg.checkPoints[index].position_r - cfg.checkPoints[index].position_l;

        if (new_center - width * 0.5f < 0) return;
        if (new_center + width * 0.5f > 1) return;

        cfg.checkPoints[index].time = time;
        cfg.checkPoints[index].position_l = new_center - width * 0.5f;
        cfg.checkPoints[index].position_r = new_center + width * 0.5f;
        this.OnPaint();
    }

    public void OnCheckPointDragLeft(PointerEventData eventData, int index)
    {
        double x = VerticalGridLine.Instance.GetPositionX(eventData, false);
        if (x > 1 || x < 0) return;
        if (x < cfg.checkPoints[index].position_r)
        {
            cfg.checkPoints[index].position_l = x;
            this.OnPaint();
        }
    }

    public void OnCheckPointDragRight(PointerEventData eventData, int index)
    {
        double x = VerticalGridLine.Instance.GetPositionX(eventData, false);
        if (x > 1 || x < 0) return;
        if (x > cfg.checkPoints[index].position_l)
        {
            cfg.checkPoints[index].position_r = x;
            this.OnPaint();
        }
    }

    /// <summary>
    /// 在最后一个检查点后添加一个新的检查点
    /// </summary>
    public void AddCheckPoint()
    {
        CheckPoint last_ckp = cfg.checkPoints[cfg.checkPoints.Count - 1];
        CheckPoint new_ckp = new CheckPoint(last_ckp);
        new_ckp.time = new_ckp.time + HorizontalGridLine.Instance.GetOneCellTime;
        cfg.checkPoints.Add(new_ckp);
        this.OnPaint();
    }
}
